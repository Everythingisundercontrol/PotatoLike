using UnityEngine;
using Yu;

namespace GameLogic.Items.Box
{
    public class BoxCtrl : MonoBehaviour, IPoolableObject
    {
        public float LastUsedTime { get; private set; }
        public bool Active { get; private set; }

        private BoxModel _model;
        private BoxView _view;

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
        /// 初始化
        /// </summary>
        public void BoxInit()
        {
            _model = new BoxModel();
            _model.Init();
            _view = gameObject.GetComponent<BoxView>();
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

            var playerCtrl = BattleManager.BattleManager.Instance.GetPlayerCtrl();
            playerCtrl.AddBox();
            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }
        }
    }
}