using System.Collections.Generic;
using GameLogic.BattleManager;
using GameLogic.Enemy.EnemyBase;
using UI.HUD.EnemyHp;
using UnityEngine;

namespace UI.Windows.Battle
{
    public class BattleModel
    {
        public float TotalTime;
        public float RemainingTime;
        
        public bool BattleEnd;
        public bool BattlePause;

        public List<EnemyBaseCtrl> EnemyBaseCtrl;

        public Dictionary<EnemyBaseCtrl, EnemyHpCtrl> EnemyHp;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInit()
        {
            BattleEnd = false;
            BattlePause = true;

            EnemyBaseCtrl = BattleManager.Instance.GetEnemyBaseCtrl();
            EnemyHp = new Dictionary<EnemyBaseCtrl, EnemyHpCtrl>();
        }

        /// <summary>
        /// 打开时
        /// </summary>
        public void OnOpen()
        {
        }
        
        
    }
}