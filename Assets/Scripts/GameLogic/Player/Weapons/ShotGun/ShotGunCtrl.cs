using System.Collections.Generic;
using GameLogic.Bullet;
using GameLogic.Player.MVC;
using GameLogic.Player.WeaponBase;
using UnityEngine;
using Yu;

namespace GameLogic.Player.Weapons.ShotGun
{
    public class ShotGunCtrl : WeaponCtrlBase
    {
        public Sprite sprite;
        public GameObject shootPos;

        private GameObject _bulletPrefab;

        private Sprite _bulletSprite;
        private BoxCollider2D _bulletBoxCollider2D;

        //属性

        private float _weaponATK = 2f;

        private float _lastAttackTime;

        private float _timeLock = 0.5f;

        private int _shootBulletNum = 6; //单枪单喷子弹数
        private float _shootAngles = 60; //散布范围

        private IEnumerable<float> ShootList => CalculateAngles(_shootBulletNum, _shootAngles);

        public override void Init()
        {
            base.Init();
            var path = ConfigManager.Tables.CfgPrefab["Bullet 5"].PrefabPath;
            _bulletPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            
            SpriteRenderer.sprite = sprite;

            _bulletSprite = _bulletPrefab.gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
            _bulletBoxCollider2D = _bulletPrefab.gameObject.GetComponentInChildren<BoxCollider2D>();
        }

        public override void WeaponFixedUpdate()
        {
            base.WeaponFixedUpdate();
            SustainedAttack();
        }

        /// <summary>
        /// 持续攻击
        /// </summary>
        private void SustainedAttack()
        {
            if (Model.IfHoldMouse && CheckTimeLock())
            {
                MultiShoot();
            }
        }

        /// <summary>
        /// 多发子弹射击,霰弹枪散射
        /// </summary>
        private void MultiShoot()
        {
            foreach (var addEulerAnglesZ in ShootList)
            {
                Shoot(addEulerAnglesZ);
            }
        }

        /// <summary>
        /// 计算射击角度
        /// </summary>
        private static IEnumerable<float> CalculateAngles(int bulletNum, float dispersionAngle)
        {
            if (bulletNum <= 0)
            {
                Debug.Log("N必须大于0");
            }


            var angles = new List<float>();

            if (bulletNum == 1)
            {
                angles.Add(0);
                return angles;
            }

            var delta = dispersionAngle / (bulletNum - 1);
            var start = -dispersionAngle / 2;

            for (var i = 0; i < bulletNum; i++)
            {
                angles.Add(start + i * delta);
            }

            return angles;
        }

        /// <summary>
        /// 子弹射击
        /// </summary>
        private void Shoot(float addEulerAnglesZ)
        {
            if (!_bulletPrefab)
            {
                return;
            }

            var bullet = PoolManager.Instance.GetObject<BulletController>();

            bullet.boxCollider2DGameObject.layer = LayerMask.NameToLayer("Player");
            bullet.boxCollider2DGameObject.tag = "Player";
            bullet.position = shootPos.transform.position;
            bullet.spriteRenderer.sprite = _bulletSprite;
            bullet.boxCollider2D = _bulletBoxCollider2D;

            var bullRotationZ = Model.Angle;
            var bullDamage = BattleManager.BattleManager.Instance.GetPlayerCtrl().GetDamage() + _weaponATK;
            const float bullBulletSpeed = 15f;
            const int bullBulletPenetrationCount = 1;
            const float bullBulletExitTime = 0.3f;
            const BulletType bullBulletType = BulletType.Player;
            
            bullet.Model.SetValue(bullRotationZ,bullDamage,bullBulletSpeed,bullBulletPenetrationCount,bullBulletExitTime,bullBulletType);
            
            var anNum = (Model.Angle + 90 + addEulerAnglesZ) * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Cos(anNum), Mathf.Sin(anNum));

            bullet.Fire(direction);

            
        }

        /// <summary>
        /// 检查时间锁
        /// </summary>
        /// <returns></returns>
        private bool CheckTimeLock()
        {
            if (!(Time.time - _lastAttackTime > _timeLock))
            {
                return false;
            }

            _lastAttackTime = Time.time;
            return true;
        }
    }
}