using System.Collections;
using GameLogic.Items.Gold;
using UI.Windows.Battle;
using UnityEngine;
using UnityEngine.AI;
using Yu;

namespace GameLogic.Enemy.EnemyBase
{
    public class EnemyBaseCtrl : MonoBehaviour, IPoolableObject
    {
        public virtual float LastUsedTime { get; protected set; }
        public virtual bool Active { get; protected set; }

        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected Animator anim;
        [SerializeField] protected Collider2D collider2d;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected GameObject rigidbody2DGameObject;
        [SerializeField] protected GameObject collider2dGameObject;
        // [SerializeField] protected TimeUser timeUser;

        protected static readonly int IsAttacked = Animator.StringToHash("isAttacked");
        private static readonly int IsDead = Animator.StringToHash("isDead");

        protected EnemyBaseModel Model;

        public virtual void OnActivate()
        {
            Active = true;
            LastUsedTime = Time.time;
            if (!gameObject)
            {
                return;
            }
            gameObject.SetActive(true);
            // EventManager.Instance.AddListener(EventName.Pause,Pause);
            // EventManager.Instance.AddListener(EventName.CancelPause,CancelPause);
            RecoverState();
        }

        public virtual void OnDeactivate()
        {
            Active = false;
            LastUsedTime = Time.time;
            gameObject.SetActive(false);
            // EventManager.Instance.RemoveListener(EventName.Pause,Pause);
            // EventManager.Instance.RemoveListener(EventName.CancelPause,CancelPause);
        }

        public virtual void OnIdleDestroy()
        {
            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void EnemyInit()
        {
            Model = new EnemyBaseModel();
            Model.Init(agent.speed, agent.acceleration);

            agent.updateUpAxis = false;
            agent.updateRotation = false;

            gameObject.SetActive(false);
        }

        /// <summary>
        /// 固定帧调用
        /// </summary>
        /// <param name="playerPosition"></param>
        public virtual void EnemyUpdate(Vector3 playerPosition)
        {
        }

        // /// <summary>
        // /// 暂停事件响应
        // /// </summary>
        // public virtual void Pause()
        // {
        // }
        //
        // /// <summary>
        // /// 解除暂停
        // /// </summary>
        // public virtual void CancelPause()
        // {
        // }

        public int GetHp()
        {
            return (int) Model.Hp;
        }

        public float GetMaxHp()
        {
            return Model.MaxHp;
        }

        /// <summary>
        /// 获取攻击力
        /// </summary>
        /// <returns></returns>
        public float GetAttack()
        {
            return Model.Attack;
        }

        /// <summary>
        /// 获取Collider2D
        /// </summary>
        /// <returns></returns>
        public Collider2D GetCollider2D()
        {
            return collider2d;
        }

        /// <summary>
        /// 受攻击
        /// </summary>
        /// <param name="damage"></param>
        public void UnderAttack(float damage)
        {
            Model.Hp -= damage;
            HpCheck();
        }

        /// <summary>
        /// 受击动画与动作
        /// </summary>
        protected virtual void UnderAttackAnim()
        {
            Debug.Log("virtual AttackedAnim()");
        }

        /// <summary>
        /// 父类固定帧调用，无参
        /// </summary>
        protected void EnemyUpdate()
        {
            ChangeFacing();
        }

        /// <summary>
        /// 受击停止后回复速度
        /// </summary>
        /// <returns></returns>
        protected IEnumerator RecoverSpeed()
        {
            yield return new WaitForSeconds(Model.AttackedWaitTime);
            anim.SetBool(IsAttacked, false);
            agent.acceleration = Model.Acceleration;
            agent.speed = Model.Speed;
            collider2d.enabled = true;
        }

        /// <summary>
        /// 死亡后池回收
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator DeadRecover()
        {
            yield return new WaitForSeconds(Model.DemiseAwaitRecycleDuration);
            PoolManager.Instance.ReturnObject(this);
        }

        /// <summary>
        /// 获取与玩家距离
        /// </summary>
        protected float GetDistance()
        {
            var dis = Vector3.Distance(transform.position, BattleManager.BattleManager.Instance.GetPlayerPosition());
            return dis;
        }

        /// <summary>
        /// 死亡处理
        /// </summary>
        private void Dead()
        {
            agent.isStopped = true;
            anim.SetBool(IsDead, true);
            collider2dGameObject.SetActive(false);
            rigidbody2DGameObject.SetActive(false);
            BattleManager.BattleManager.Instance.RemoveEnemyControllerFromList(this);
            LootDrop();
            StartCoroutine(DeadRecover());
        }

        /// <summary>
        /// 敌人死亡掉落，爆金币
        /// </summary>
        private void LootDrop()
        {
            var gc = PoolManager.Instance.GetObject<GoldCtrl>();
            gc.SetGold(transform.position, 2);
            gc.SetGoldValue(3);
        }

        /// <summary>
        /// 血条检查
        /// </summary>
        private void HpCheck()
        {
            if (Model.Hp <= 0)
            {
                Dead();
                return;
            }

            UnderAttackAnim();
        }

        /// <summary>
        /// 改变敌人朝向
        /// </summary>
        private void ChangeFacing()
        {
            if (agent.velocity.x < 0 && !Model.FaceToLeft)
            {
                anim.gameObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
                Model.FaceToLeft = true;
                return;
            }

            if (!(agent.velocity.x > 0) || !Model.FaceToLeft)
            {
                return;
            }

            anim.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            Model.FaceToLeft = false;
        }

        /// <summary>
        /// 从状态池中取出后重置状态
        /// </summary>
        private void RecoverState()
        {
            rigidbody2DGameObject.SetActive(true);
            collider2dGameObject.SetActive(true);
            Model.RecoverState();
        }
    }
}