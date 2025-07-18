using System.Collections;
using System.Collections.Generic;
using GameLogic.Bullet;
using GameLogic.Enemy.EnemyBase;
using UnityEngine;
using Yu;

namespace GameLogic.Enemy.Enemys.TurretEnemy
{
    public class TurretEnemyCtrl : EnemyBaseCtrl
    {
        [SerializeField] private List<GameObject> bulletPos;

        private TurretEnemyModel _model;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void EnemyInit()
        {
            base.EnemyInit();

            _model = new TurretEnemyModel();
            _model.Init();
        }

        /// <summary>
        /// 固定帧调用
        /// </summary>
        /// <param name="playerPosition"></param>
        public override void EnemyUpdate(Vector3 playerPosition)
        {
            AttackAbleCheck();
        }
        
        public override void OnIdleDestroy()
        {
            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }
            Destroy(gameObject);
        }
        
        /// <summary>
        /// 死亡后池回收
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator DeadRecover()
        {
            yield return new WaitForSeconds(Model.DemiseAwaitRecycleDuration);
            PoolManager.Instance.ReturnObject(this);
        }

        /// <summary>
        /// 受击动画与动作
        /// </summary>
        protected override void UnderAttackAnim()
        {
            if (anim.GetBool(IsAttacked))
            {
                //todo:应该延长，但先不管
                return;
            }

            anim.SetBool(IsAttacked, true);
            agent.acceleration = 100;
            agent.speed = 0;
            collider2d.enabled = false;
            StartCoroutine(RecoverSpeed());
        }

        /// <summary>
        /// 检测是否能攻击
        /// </summary>
        private void AttackAbleCheck()
        {
            if (Time.time - _model.LastAttackTime < _model.AttackedCoolDown)
            {
                return;
            }

            if (anim.GetBool(IsAttacked))
            {
                return;
            }

            StartCoroutine(PreAttackAnim());
            _model.LastAttackTime = Time.time;
        }

        /// <summary>
        /// 攻击前摇动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator PreAttackAnim()
        {
            var oldColor = spriteRenderer.color;
            var attackedColor = new Color(1, 0, 0);
            spriteRenderer.color = attackedColor;

            yield return new WaitForSeconds(_model.PREAttackAnimation);

            spriteRenderer.color = oldColor;

            Attack();
        }

        /// <summary>
        /// 攻击
        /// </summary>
        private void Attack()
        {
            if (_model.BulletList.Count <= 0)
            {
                return;
            }

            var bullet = PoolManager.Instance.GetObject<BulletController>();
            bullet.boxCollider2D.gameObject.layer = LayerMask.NameToLayer("Enemy");
            bullet.boxCollider2D.gameObject.tag = "Enemy";
            bullet.position = bulletPos[1].transform.position;
            var playerPos = BattleManager.BattleManager.Instance.GetPlayerPosition();
            var direction = (playerPos - bullet.position).normalized;

            var randomNum = Random.Range(0, _model.BulletList.Count);
            
            bullet.spriteRenderer.sprite = _model.BulletList[randomNum].Item2;
            bullet.boxCollider2D = _model.BulletList[randomNum].Item3;
            
            var bullRotationZ = Vector2.Angle(playerPos - bullet.position, Vector2.up);
            if (playerPos.x > bullet.position.x)
            {
                bullRotationZ = -bullRotationZ;
            }

            var bullDamage = Model.Attack + _model.BulletList[randomNum].Item1;
            const float bullBulletSpeed = 10f;
            const int bullBulletPenetrationCount = 1;
            const float bullBulletExitTime = 10f;
            const BulletType bullBulletType = BulletType.Enemy;

            bullet.Model.SetValue(bullRotationZ,bullDamage,bullBulletSpeed,bullBulletPenetrationCount,bullBulletExitTime,bullBulletType);
            
            bullet.Fire(direction);
        }
    }
}