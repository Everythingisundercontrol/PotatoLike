using System.Collections.Generic;
using UnityEngine;
using Yu;

namespace GameLogic.Enemy.Enemys.TurretEnemy
{
    public class TurretEnemyModel
    {
        public List<(int, Sprite, BoxCollider2D)> BulletList;

        public float LastAttackTime;
        public float AttackedCoolDown;
        public float PREAttackAnimation;
        
        private List<string> _bulletPathKey;

        public void Init()
        {
            AttackedCoolDown = 1.5f;
            PREAttackAnimation = 0.5f;
            
            BulletList = new List<(int/* 攻击力 */, Sprite, BoxCollider2D)>();
            _bulletPathKey = new List<string> //子弹预制体路径列表
            {
                "EnemyBullet1","EnemyBullet2","EnemyBullet3"
            };

            foreach (var key in _bulletPathKey)
            {
                var path = ConfigManager.Tables.CfgPrefab[key].PrefabPath;
                var bulletPrefab = AssetManager.Instance.LoadAssetGameObject(path);

                var bulletSprite = bulletPrefab.gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
                var bulletBoxCollider2D = bulletPrefab.gameObject.GetComponentInChildren<BoxCollider2D>();
                BulletList.Add((1, bulletSprite, bulletBoxCollider2D));
            }
        }
    }
}