using System.Collections;
using GameLogic.Enemy.EnemyBase;
using UnityEngine;
using Yu;

namespace GameLogic.Enemy.Enemys.SpikerEnemy
{
    public class SpikerEnemyCtrl : EnemyBaseCtrl
    {
        // private SpikerEnemyModel _model;

        /// <summary>
        /// 固定帧调用
        /// </summary>
        /// <param name="playerPosition"></param>
        public override void EnemyUpdate(Vector3 playerPosition)
        {
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
    }
}