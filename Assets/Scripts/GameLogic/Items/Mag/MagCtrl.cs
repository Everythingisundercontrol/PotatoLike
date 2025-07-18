using UnityEngine;
using Yu;

namespace GameLogic.Items.Mag
{
    public class MagCtrl: MonoBehaviour, IPoolableObject
    {
        public float LastUsedTime { get; private set; }
        public bool Active { get; private set; }

        private MagModel _model;
        private MagView _view;

        public void OnActivate()
        {
            Active = true;
            LastUsedTime = Time.time;
            gameObject.SetActive(true);
        }

        public void OnDeactivate()
        {
            Active = false;
            LastUsedTime = Time.time;
            gameObject.SetActive(false);
        }

        public void OnIdleDestroy()
        {
            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// 磁铁初始化
        /// </summary>
        public void Init()
        {
            _model = new MagModel();
            _model.Init();
            _view = gameObject.GetComponent<MagView>();
            _view.Init();
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 被玩家碰撞
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.tag.Equals("Player"))
            {
                return;
            }

            var playerController = BattleManager.BattleManager.Instance.GetPlayerCollider();

            if (other != playerController)
            {
                return;
            }
            
            //事件调用,磁吸
            EventManager.Instance.Dispatch(EventName.MagGetGold);

            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }
        }
    }
}