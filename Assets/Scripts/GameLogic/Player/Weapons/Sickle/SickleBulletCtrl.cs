using System;
using System.Collections;
using GameLogic.Enemy.EnemyBase;
using GameLogic.Enemy.Enemys.BasicEnemy;
using UnityEngine;
using Yu;

namespace GameLogic.Player.Weapons.Sickle
{
    public class SickleBulletCtrl : MonoBehaviour, IPoolableObject
    {
        public Vector3 Position; //

        public SpriteRenderer SpriteRenderer;

        public Rigidbody2D Rigidbody2D;

        public BoxCollider2D BoxCollider2D; //子弹碰撞箱
        
        public GameObject boxCollider2DGameObject;

        public float rotationZ; //子弹旋转角度

        public float damage; //伤害

        public float bulletSpeed = 5f; //子弹初始速度
        public float bulletSpeedDecayRate = 0.05f; //子弹速度衰减率
        public float speedJumpEdge = 0.1f; //子弹速度跳跃临界值

        // public float bulletPenetrationCount; //子弹穿透次数

        public float bulletExitTime = 6f; //子弹存续时间

        public float rotateSpeed = 360f; //初始旋转速度
        public float rotateSpeedGrowthRate = 0.05f; //旋转速度增长率
        public float rotateSpeedMax = 720f; //最大旋转速度
        public float rotateJumpEdge = 10f; //旋转速度跳跃临界值
        
        public float LastUsedTime { get; private set; } // 对象上一次使用的时间，交由PoolManager进行自动销毁算法判断
        public bool Active { get; private set; } // 是否激活中，OnActivate()和OnDeactivate()会进行修改

        private float _bulletSpeed; //子弹速度
        private Vector2 _direction; //子弹方向
        private float _rotateSpeed; //子弹当前旋转速度

        public void OnActivate() // 激活时
        {
            Active = true;
            LastUsedTime = Time.time;
            gameObject.SetActive(true);
        }

        public void OnDeactivate() // 主动归还时
        {
            Active = false;
            LastUsedTime = Time.time;
            gameObject.SetActive(false);
            EventManager.Instance.Dispatch(EventName.RemoveSickleBulletCtrl,this);
        }

        public void OnIdleDestroy() // PoolManager自动销毁对象时
        {
            if (!Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }

            Destroy(gameObject);
        }

        public void FixedUpdate()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (_bulletSpeed != 0)
            {
                SetMoveSpeed();
            }

            SetRotateSpeed();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 开火时，设置子弹位置角度与速度
        /// </summary>
        public void Fire(Vector2 direction)
        {
            _direction = direction;
            _bulletSpeed = bulletSpeed;
            _rotateSpeed = rotateSpeed;
            var thisTransform = transform;
            thisTransform.position = Position;
            thisTransform.eulerAngles = new Vector3(0, 0, rotationZ);
            Rigidbody2D.velocity = direction * bulletSpeed;
            gameObject.SetActive(true);

            if (bulletExitTime > 0)
            {
                StartCoroutine(BulletReturnByTime());
            }
        }

        /// <summary>
        /// 子弹到达时间上限自动回池
        /// </summary>
        /// <returns></returns>
        private IEnumerator BulletReturnByTime()
        {
            yield return new WaitForSeconds(bulletExitTime);
            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }
        }

        /// <summary>
        /// 速度插值减慢
        /// </summary>
        private void SetMoveSpeed()
        {
            if (_bulletSpeed <= bulletSpeed * speedJumpEdge)
            {
                _bulletSpeed = 0;
            }
            
            _bulletSpeed = Mathf.Lerp(_bulletSpeed, 0, bulletSpeedDecayRate);
            Rigidbody2D.velocity = _bulletSpeed * _direction;
        }

        /// <summary>
        /// 旋转速度插值变快
        /// </summary>
        private void SetRotateSpeed()
        {
            if (Math.Abs(_rotateSpeed - rotateSpeedMax) < rotateJumpEdge)
            {
                transform.Rotate(0, 0, rotateSpeedMax * Time.deltaTime);
                return;
            }

            _rotateSpeed = Mathf.Lerp(_rotateSpeed, rotateSpeedMax, rotateSpeedGrowthRate);
            transform.Rotate(0, 0, _rotateSpeed * Time.deltaTime);
        }

        /// <summary>
        /// 检测是否射到敌人
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.tag.Equals("Enemy"))
            {
                return;
            }

            var enemyCtrl = BattleManager.BattleManager.Instance.TryGetEnemyCtrl(other);
            if (!enemyCtrl)
            {
                // AttackEnemyBullet();
                Debug.Log("砍到敌人子弹");
                return;//todo:敌人子弹
            }
            ShootEnemyEvent(enemyCtrl);
        }

        /// <summary>
        /// 子弹射击到敌人事件   //todo:攻击敌人然后子弹消失，特效
        /// </summary>
        private void ShootEnemyEvent(EnemyBaseCtrl enemyController)
        {
            if (!Active)
            {
                return;
            }

            enemyController.UnderAttack(damage);

            // bulletPenetrationCount--;
            // if (bulletPenetrationCount <= 0)
            // {
            //     PoolManager.Instance.ReturnObject(this);
            // }
        }
        
        // /// <summary>
        // /// 砍到敌人子弹加钱
        // /// </summary>
        // private void AttackEnemyBullet()
        // {
        //     var pc = BattleManager.BattleManager.Instance.GetPlayerCtrl();
        //     pc.AddMoney(1);
        //     Debug.Log("Money : " + pc.GetMoney());
        // }
    }
}