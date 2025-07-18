using System;
using UnityEngine;
using Yu;

namespace GameLogic.Items.Gold
{
    public class GoldCtrl : MonoBehaviour, IPoolableObject
    {
        public float LastUsedTime { get; private set; }
        public bool Active { get; private set; }

        private GoldModel _model;
        private GoldView _view;

        public void OnActivate()
        {
            Active = true;
            LastUsedTime = Time.time;
            gameObject.SetActive(true);
            EventManager.Instance.AddListener(EventName.MagGetGold, AgentToPlayer);
        }

        public void OnDeactivate()
        {
            Active = false;
            LastUsedTime = Time.time;
            gameObject.SetActive(false);
            EventManager.Instance.RemoveListener(EventName.MagGetGold, AgentToPlayer);
        }

        public void OnIdleDestroy()
        {
            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }

            Destroy(gameObject);
        }

        public void FixedUpdate()
        {
            if (_model.IfMag == false)
            {
                return;
            }

            var pos = BattleManager.BattleManager.Instance.GetPlayerPosition();
            _view.agent.SetDestination(pos);
        }

        /// <summary>
        /// 金币初始化
        /// </summary>
        public void GoldInit()
        {
            _model = new GoldModel();
            _model.Init();
            _view = gameObject.GetComponent<GoldView>();
            _view.Init();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 向玩家移动
        /// </summary>
        public void AgentToPlayer()
        {
            _view.agent.isStopped = false;
            _model.IfMag = true;
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        public void StopAgent()
        {
            _model.IfMag = false;
            _view.agent.isStopped = true;
        }

        /// <summary>
        /// 设置金币的属性
        /// </summary>
        public void SetGold(Vector3 position, int num)
        {
            transform.position = position;
            _view.SetSprite(num);
        }

        /// <summary>
        /// 设置金币价值
        /// </summary>
        public void SetGoldValue(int num)
        {
            _model.GoldValue = num;
        }

        /// <summary>
        /// 获取金币价值
        /// </summary>
        /// <returns></returns>
        public int GetGoldValue()
        {
            return _model.GoldValue;
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
            playerCtrl.AddMoney(_model.GoldValue);
            StopAgent();
            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }
        }
    }
}