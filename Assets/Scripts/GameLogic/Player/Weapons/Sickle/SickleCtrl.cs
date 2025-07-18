using System.Collections.Generic;
using GameLogic.Bullet;
using GameLogic.Enemy.EnemyBase;
using GameLogic.Player.MVC;
using GameLogic.Player.WeaponBase;
using Unity.Mathematics;
using UnityEngine;
using Yu;

namespace GameLogic.Player.Weapons.Sickle
{
    public class SickleCtrl : WeaponCtrlBase
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

        private List<SickleBulletCtrl> _sickleBulletCtrlList;

        //属性

        private float _weaponATK = 2f; //武器附加攻击力

        private float _lastAttackTime; //上次攻击时间

        private float _timeLock = 0.5f; //最短攻击时间间隔

        private static readonly int AttackAnim = Animator.StringToHash("SickleAttackAnim");

        public override void Init()
        {
            base.Init();

            _originalScale = transform.parent.transform.localScale;

            SpriteRenderer.sprite = sprite;
            _anim = Animator;

            _sickleBulletCtrlList = new List<SickleBulletCtrl>();

            BindEvent();
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
            EventManager.Instance.RemoveListener<SickleBulletCtrl>(EventName.RemoveSickleBulletCtrl, RemoveSickleBulletCtrl);
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
        /// 事件绑定
        /// </summary>
        private void BindEvent()
        {
            EventManager.Instance.AddListener<SickleBulletCtrl>(EventName.RemoveSickleBulletCtrl, RemoveSickleBulletCtrl);
        }

        /// <summary>
        /// 移除sickleBulletCtrl
        /// </summary>
        /// <param name="sickleBulletCtrl"></param>
        private void RemoveSickleBulletCtrl(SickleBulletCtrl sickleBulletCtrl)
        {
            if (!_sickleBulletCtrlList.Contains(sickleBulletCtrl))
            {
                return;
            }

            _sickleBulletCtrlList.Remove(sickleBulletCtrl);
        }

        /// <summary>
        /// 检查SickleBulletCtrlList的count是否超过num，是则回收最久的SickleBulletCtrl
        /// </summary>
        private void CheckSickleBulletCtrlList(int num)
        {
            if (_sickleBulletCtrlList.Count <= num)
            {
                return;
            }

            PoolManager.Instance.ReturnObject(_sickleBulletCtrlList[0]);
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

            if (CheckTimeLock())
            {
                Attack();
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
                return; //todo:敌人子弹
            }

            Attack(enemyCtrl);
        }

        /// <summary>
        /// 攻击生成小镰刀
        /// </summary>
        private void Attack()
        {
            CheckSickleBulletCtrlList(3);
            var sickleBullet = PoolManager.Instance.GetObject<SickleBulletCtrl>();
            sickleBullet.boxCollider2DGameObject.layer = LayerMask.NameToLayer("Player");
            sickleBullet.boxCollider2DGameObject.tag = "Player";
            sickleBullet.Position = transform.position;
            sickleBullet.rotationZ = Model.Angle;

            var anNum = (Model.Angle + 90) * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Cos(anNum), Mathf.Sin(anNum));

            sickleBullet.Fire(direction);
            sickleBullet.damage = BattleManager.BattleManager.Instance.GetPlayerCtrl().GetDamage() + _weaponATK;
            _sickleBulletCtrlList.Add(sickleBullet);
        }

        /// <summary>
        /// 普通攻击
        /// </summary>
        /// <param name="enemyCtrl"></param>
        private void Attack(EnemyBaseCtrl enemyCtrl)
        {
            var damage = BattleManager.BattleManager.Instance.GetPlayerCtrl().GetDamage() + _weaponATK;
            enemyCtrl.UnderAttack(damage);
        }
    }
}