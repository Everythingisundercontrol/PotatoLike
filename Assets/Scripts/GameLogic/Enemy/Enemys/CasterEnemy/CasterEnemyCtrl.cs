using System.Collections;
using System.Collections.Generic;
using GameLogic.Bullet;
using GameLogic.Enemy.EnemyBase;
using UnityEngine;
using UnityEngine.AI;
using Yu;

namespace GameLogic.Enemy.Enemys.CasterEnemy
{
    public class CasterEnemyCtrl : EnemyBaseCtrl
    {
        [SerializeField] private List<GameObject> bulletPos; //子弹生成位置

        private CasterEnemyModel _model;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void EnemyInit()
        {
            base.EnemyInit();

            _model = new CasterEnemyModel();
            _model.Init();
        }

        /// <summary>
        /// 固定帧调用
        /// </summary>
        /// <param name="playerPosition"></param>
        public override void EnemyUpdate(Vector3 playerPosition)
        {
            base.EnemyUpdate();

            Move(playerPosition);
            AttackAbleCheck();
        }

        // /// <summary>
        // /// 暂停
        // /// </summary>
        // public override void Pause()
        // {
        //     base.Pause();
        //     _model.FirstPauseCheck = true;
        //     _model.PauseTime = Time.time;
        //     _model.RemainingAttackTime = _model.AttackedCoolDown - (_model.PauseTime - _model.LastAttackTime);
        // }
        //
        // /// <summary>
        // /// 取消暂停
        // /// </summary>
        // public override void CancelPause()
        // {
        //     base.CancelPause();
        //     _model.CancelPauseTime = Time.time;
        // }

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
        /// 移动逻辑
        /// </summary>
        private void Move(Vector3 playerPosition)
        {
            var dis = Vector3.Distance(transform.position, playerPosition);
            if (dis <= _model.AvoidDistance)
            {
                Avoid(); //执行回避逻辑
                return;
            }

            if (_model.IsRandomMove)
            {
                return;
            }

            StartCoroutine(RandomMove()); //执行随机游荡逻辑
        }

        /// <summary>
        /// 回避逻辑
        /// </summary>
        private void Avoid()
        {
            var position = transform.position;
            var direction = (position - BattleManager.BattleManager.Instance.GetPlayerPosition()).normalized;
            var goalPos = position + direction * _model.AvoidStepDistance;

            if (!NavMesh.SamplePosition(goalPos, out var hit, _model.AvoidDistance, NavMesh.AllAreas))
            {
                return;
            }

            var randomGoal = hit.position;
            agent.SetDestination(randomGoal);
        }

        /// <summary>
        /// 随机移动
        /// </summary>
        private IEnumerator RandomMove()
        {
            _model.IsRandomMove = true;
            var randomDirection = Random.insideUnitSphere * 30;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out var hit, 3, NavMesh.AllAreas))
            {
                var randomGoal = hit.position;
                agent.SetDestination(randomGoal);
            }

            yield return null;
            _model.IsRandomMove = false;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 检查是否能攻击
        /// </summary>
        private void AttackAbleCheck()
        {
            // if (!CheckPause())
            // {
            //     return;
            // }

            if (Time.time - _model.LastAttackTime < _model.AttackedCoolDown)
            {
                return;
            }

            if (anim.GetBool(IsAttacked))
            {
                return;
            }

            // Debug.Log("AttackAbleCheck()");
            StartCoroutine(PreAttackAnim());
            _model.LastAttackTime = Time.time;
            // _model.FirstPauseCheck = false;
        }

        // /// <summary>
        // /// 检查暂停时相关的攻击行为是否执行，以及暂停后攻击的恢复     //todo:
        // /// </summary>
        // /// <returns></returns>
        // private bool CheckPause()
        // {
        //     if (timeUser.timeScale == 0)
        //     {
        //         return false;
        //     }
        //
        //     // if (_model.FirstPauseCheck)
        //     // {
        //     //     Debug.Log(Time.time - _model.CancelPauseTime+" :: "+_model.RemainingAttackTime);
        //     //     return Time.time - _model.CancelPauseTime < _model.RemainingAttackTime;
        //     // }
        //     
        //     return Time.time - _model.LastAttackTime < _model.AttackedCoolDown;
        // }

        /// <summary>
        /// 攻击动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator PreAttackAnim()
        {
            // Debug.Log("PreAttackAnim");
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
            bullet.boxCollider2DGameObject.layer = LayerMask.NameToLayer("Enemy");
            bullet.boxCollider2DGameObject.tag = "Enemy";
            bullet.position = bulletPos[0].transform.position;
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

            bullet.Model.SetValue(bullRotationZ, bullDamage, bullBulletSpeed, bullBulletPenetrationCount, bullBulletExitTime, bullBulletType);

            bullet.Fire(direction);
        }
    }
}