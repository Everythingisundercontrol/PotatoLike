using System.Collections.Generic;
using UnityEngine;
using Yu;

namespace GameLogic.Enemy.Enemys.CasterEnemy
{
    public class CasterEnemyModel
    {
        public float AvoidDistance; //回避距离
        public float AvoidStepDistance; //回避步长
        public bool IsRandomMove; //是否在随机移动中

        public List<(int, Sprite, BoxCollider2D)> BulletList;

        // public bool FirstPauseCheck;    //取消暂停后的第一个攻击
        // public float PauseTime; //暂停的时间点
        // public float CancelPauseTime;   //取消暂停的时间点，放到BattleManager里面
        // public float RemainingAttackTime;   //剩余时间，AttackedCoolDown减去（暂停时间点减去LastAttackTime）
        
        public float LastAttackTime;
        public float AttackedCoolDown;
        public float PREAttackAnimation;

        private List<string> _bulletPathKey;

        /// <summary>
        /// 数据初始化
        /// </summary>
        public void Init()
        {
            AvoidDistance = 10;
            AvoidStepDistance = 3;
            IsRandomMove = false;

            AttackedCoolDown = 1.5f;
            PREAttackAnimation = 0.5f;

            BulletList = new List<(int /* 攻击力 */, Sprite, BoxCollider2D)>();
            _bulletPathKey = new List<string> //子弹预制体路径列表
            {
                "EnemyBullet1", "EnemyBullet2", "EnemyBullet3"
            };

            foreach (var key in _bulletPathKey)
            {
                var path = ConfigManager.Tables.CfgPrefab[key].PrefabPath;
                var bulletPrefab = AssetManager.Instance.LoadAssetGameObject(path);

                var bulletSprite = bulletPrefab.gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
                var bulletBoxCollider2D = bulletPrefab.gameObject.GetComponentInChildren<BoxCollider2D>();
                BulletList.Add((1, bulletSprite, bulletBoxCollider2D));
            }

            // FirstPauseCheck = false;
        }
        
    }
}