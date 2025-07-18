using GameLogic.Bullet;
using GameLogic.Player.WeaponBase;
using UnityEngine;
using Yu;

namespace GameLogic.Player.Weapons.MachineGun
{
    public class MachineGunCtrl : WeaponCtrlBase
    {
        public Sprite sprite;
        public GameObject shootPos;

        private GameObject _bulletPrefab;

        private Sprite _bulletSprite;
        private BoxCollider2D _bulletBoxCollider2D;

        //属性

        private float _weaponATK = 0f;

        private float _lastAttackTime;

        private float _timeLock = 0.1f;

        public override void Init()
        {
            base.Init();

            var path = ConfigManager.Tables.CfgPrefab["Bullet 4"].PrefabPath;
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
                Shoot();
            }
        }

        /// <summary>
        /// 子弹射击
        /// </summary>
        private void Shoot()
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
            const float bullBulletSpeed = 28f;
            const int bullBulletPenetrationCount = 1;
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
            if (!(Time.time - _lastAttackTime > _timeLock))
            {
                return false;
            }

            _lastAttackTime = Time.time;
            return true;
        }
    }
}