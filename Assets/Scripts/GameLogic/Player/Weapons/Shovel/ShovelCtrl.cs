using GameLogic.Enemy.EnemyBase;
using GameLogic.Enemy.Enemys.BasicEnemy;
using GameLogic.Player.MVC;
using GameLogic.Player.WeaponBase;
using Unity.VisualScripting;
using UnityEngine;
using Yu;

namespace GameLogic.Player.Weapons.Shovel
{
    public class ShovelCtrl : WeaponCtrlBase
    {
        public Sprite sprite;
        public BoxCollider2D boxCollider2D;

        public float ScaleNum
        {
            get => _scaleNum;
            set
            {
                ChangeScale(value);
                _scaleNum = value;
            }
        }

        private Animator _anim;

        private Vector3 _originalScale;
        private float _scaleNum;

        //属性

        private float _weaponATK = 0f; //武器附加攻击力

        private float _lastAttackTime; //上次攻击时间

        private float _timeLock = 0.2f; //最短攻击时间间隔


        private static readonly int AttackAnim = Animator.StringToHash("ShovelAttackAnim");

        public override void Init()
        {
            base.Init();
            
            _originalScale = transform.parent.transform.localScale;

            SpriteRenderer.sprite = sprite;
            _anim = Animator;
        }

        public override void WeaponFixedUpdate()
        {
            base.WeaponFixedUpdate();
            SustainedAttack();
        }

        public override void Quit()
        {
            base.Quit();
            _anim.SetBool(AttackAnim, false);
            transform.parent.transform.localScale = _originalScale;
        }
        
        /// <summary>
        /// 设置ScaleNum
        /// </summary>
        /// <param name="scaleNum"></param>
        public override void SetScaleNum(float scaleNum)
        {
            ScaleNum = scaleNum;
        }

        /// <summary>
        /// 改变Scale大小
        /// </summary>
        /// <param name="scaleNum"></param>
        private void ChangeScale(float scaleNum)
        {
            transform.parent.transform.localScale = _originalScale * scaleNum;
        }

        /// <summary>
        /// 检查时间锁
        /// </summary>
        /// <returns></returns>
        private bool CheckTimeLock()
        {
            if (!(Time.time - _lastAttackTime > _timeLock))
            {
                return false;
            }

            _lastAttackTime = Time.time;
            return true;
        }

        /// <summary>
        /// 持续攻击
        /// </summary>
        private void SustainedAttack()
        {
            if (!Model.IfHoldMouse)
            {
                if (_anim.GetBool(AttackAnim))
                {
                    _anim.SetBool(AttackAnim, false);
                }

                return;
            }

            if (!_anim.GetBool(AttackAnim))
            {
                _anim.SetBool(AttackAnim, true);
            }
        }

        /// <summary>
        /// 检测是否砍到敌人
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.gameObject.tag.Equals("Enemy"))
            {
                return;
            }

            if (!Model.IfHoldMouse)
            {
                return;
            }

            if (!CheckTimeLock())
            {
                return;
            }

            var enemyCtrl = BattleManager.BattleManager.Instance.TryGetEnemyCtrl(other);
            if (!enemyCtrl)
            {
                return;//todo:砍到敌人子弹
            }
            Attack(enemyCtrl);
        }

        /// <summary>
        /// 攻击
        /// </summary>
        private void Attack(EnemyBaseCtrl enemyCtrl)
        {
            var damage = BattleManager.BattleManager.Instance.GetPlayerCtrl().GetDamage() + _weaponATK;
            enemyCtrl.UnderAttack(damage);
        }
    }
}