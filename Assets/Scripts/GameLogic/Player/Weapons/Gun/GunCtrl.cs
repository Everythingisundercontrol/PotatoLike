using GameLogic.Player.WeaponBase;
using GameLogic.Bullet;
using UnityEngine;
using Yu;

namespace GameLogic.Player.Weapons.Gun
{
    public class GunCtrl : WeaponCtrlBase
    {
        public Sprite sprite;
        public GameObject shootPos;

        public GameObject bulletPrefab;

        public Sprite bulletSprite;
        public BoxCollider2D bulletBoxCollider2D;
        
        private GunModel _model;

        public override void Init()
        {
            base.Init();
            _model = new GunModel();
            _model.Init();
            
            var path = ConfigManager.Tables.CfgPrefab["Bullet 3"].PrefabPath;
            bulletPrefab = AssetManager.Instance.LoadAssetGameObject(path);

            bulletSprite = bulletPrefab.gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
            bulletBoxCollider2D = bulletPrefab.gameObject.GetComponentInChildren<BoxCollider2D>();
            
            SpriteRenderer.sprite = sprite;
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
                Shoot();
            }
        }

        /// <summary>
        /// 子弹射击
        /// </summary>
        private void Shoot()
        {
            if (!bulletPrefab)
            {
                return;
            }

            var bullet = PoolManager.Instance.GetObject<BulletController>();
            
            bullet.boxCollider2DGameObject.layer = LayerMask.NameToLayer("Player");
            bullet.boxCollider2DGameObject.tag = "Player";
            bullet.position = shootPos.transform.position;
            bullet.spriteRenderer.sprite = bulletSprite;
            bullet.boxCollider2D = bulletBoxCollider2D;
            
            var bullRotationZ = Model.Angle;
            var bullDamage = BattleManager.BattleManager.Instance.GetPlayerCtrl().GetDamage() + _model.WeaponAtk;
            const float bullBulletSpeed = 35f;
            const int bullBulletPenetrationCount = 2;
            const float bullBulletExitTime = -1;
            const BulletType bullBulletType = BulletType.Player;
            
            bullet.Model.SetValue(bullRotationZ,bullDamage,bullBulletSpeed,bullBulletPenetrationCount,bullBulletExitTime,bullBulletType);

            var anNum = (Model.Angle + 90) * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Cos(anNum), Mathf.Sin(anNum));

            bullet.Fire(direction);
        }

        /// <summary>
        /// 检查时间锁
        /// </summary>
        /// <returns></returns>
        private bool CheckTimeLock()
        {
            if (Time.time - _model.LastAttackTime <= _model.TimeLock)
            {
                return false;
            }

            _model.LastAttackTime = Time.time;
            return true;
        }
    }
}