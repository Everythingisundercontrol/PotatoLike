using System;
using System.Collections.Generic;
using GameLogic.Enemy.EnemyBase;
using GameLogic.Enemy.Enemys.BasicEnemy;
using GameLogic.Player.MVC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic.BattleManager
{
    public class BattleManagerModel
    {
        public GameObject PlayerGameObject;
        public PlayerController PlayerController;
        public Collider2D PlayerCollCollider2D;

        public List<EnemyBaseCtrl> EnemyBaseControllers;

        public Vector3 Po1;
        public Vector3 Po3;

        public Dictionary<string, Func<EnemyBaseCtrl>> EnemyGenerateActions;
        public Dictionary<Collider2D, EnemyBaseCtrl> Collider2EnemyCtrl;

        public float SummonInterval;

        public bool IfPause;

        public void Init()
        {
            EnemyBaseControllers = new List<EnemyBaseCtrl>();
            SummonInterval = 100f;
            IfPause = false;
            EnemyGenerateActions = new Dictionary<string, Func<EnemyBaseCtrl>>();
            Collider2EnemyCtrl = new Dictionary<Collider2D, EnemyBaseCtrl>();
        }

        /// <summary>
        /// 获取地图上的随机点位
        /// </summary>
        public Vector3 GetRandomPosition()
        {
            var randomX = Random.Range(Po3.x, Po1.x);
            var randomY = Random.Range(Po3.y, Po1.y);

            return new Vector3(randomX, randomY,0);
        }

        /// <summary>
        /// 设置生成间隔
        /// </summary>
        public void SetSummonInterval()
        {
            if (EnemyBaseControllers.Count >= 10)
            {
                SummonInterval = 100;
            }
        }
    }
}