using System.Collections;
using GameLogic.Enemy.EnemyBase;
using UnityEngine;
using Yu;

namespace GameLogic.Enemy.Enemys.DashEnemy
{
    public class DashEnemyCtrl : EnemyBaseCtrl
    {
        private DashEnemyModel _model;
        private DashEnemyView _view;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void EnemyInit()
        {
            base.EnemyInit();
            _model = new DashEnemyModel();
            _model.Init();
        }

        /// <summary>
        /// 固定帧调用
        /// </summary>
        /// <param name="playerPosition"></param>
        public override void EnemyUpdate(Vector3 playerPosition)
        {
            base.EnemyUpdate();
            Move(playerPosition); //设置目标位置
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
        /// 受击动画与动作//todo:冲刺时有受击动画但是冲刺不停止
        /// </summary>
        protected override void UnderAttackAnim()
        {
            if (_model.IsDash)
            {
                anim.SetBool(IsAttacked, true);
                StartCoroutine(DashRecoverAttackedAnim());
                return;
            }

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

        #region 移动与冲刺

        /// <summary>
        /// 移动接近
        /// </summary>
        /// <param name="playerPosition"></param>
        private void Move(Vector3 playerPosition)
        {
            if (GetDistance() < _model.DashDistance)
            {
                if (!_model.IsDash && DashAbleCheck())
                {
                    StartCoroutine(Dash());
                    return;
                }
            }

            if (_model.IsDash)
            {
                return;
            }

            agent.SetDestination(playerPosition);
        }

        /// <summary>
        /// 冲刺
        /// </summary>
        private IEnumerator Dash()
        {
            _model.IsDash = true;
            agent.isStopped = true;
            agent.speed = 0;
            agent.autoBraking = false;

            _model.DashGoal = BattleManager.BattleManager.Instance.GetPlayerPosition();
            var start = transform.position;
            _model.DashGoal.z = 0;
            start.z = 0;
            _model.DashDirection = (_model.DashGoal - start).normalized;

            var oldColor = spriteRenderer.color;
            var attackedColor = new Color(1, 0, 0);
            spriteRenderer.color = attackedColor;

            yield return new WaitForSeconds(_model.PREDashAnimation);

            spriteRenderer.color = oldColor;

            agent.isStopped = false;
            agent.speed = _model.DashSpeed;
            var time = 0f;
            while (time < _model.DashTimeConsuming)
            {
                agent.SetDestination(transform.position + _model.DashDirection);
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            agent.autoBraking = true;
            agent.speed = Model.Speed;
            _model.IsDash = false;
            _model.LastDashTime = Time.time;
        }

        /// <summary>
        /// 检查是否能触发dash
        /// </summary>
        /// <returns></returns>
        private bool DashAbleCheck()
        {
            return !(Time.time - _model.LastDashTime < _model.DashCooldown);
        }

        #endregion

        /// <summary>
        /// 冲刺时受击后回复正常动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator DashRecoverAttackedAnim()
        {
            yield return new WaitForSeconds(Model.AttackedWaitTime);
            anim.SetBool(IsAttacked, false);
        }
    }
}