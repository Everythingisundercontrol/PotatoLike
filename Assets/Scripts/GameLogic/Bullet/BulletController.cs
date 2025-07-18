using System;
using System.Collections;
using GameLogic.Enemy.EnemyBase;
using GameLogic.Player.MVC;
using UnityEngine;
using Yu;

namespace GameLogic.Bullet
{
    public class BulletController : MonoBehaviour, IPoolableObject
    {
        public Vector3 position; //

        public SpriteRenderer spriteRenderer;

        public Rigidbody2D bulletRigidbody2D;

        public BoxCollider2D boxCollider2D; //子弹碰撞箱

        public GameObject boxCollider2DGameObject;

        public TimeUser timeUser;
        
        public BulletModel Model;

        public float LastUsedTime { get; private set; } // 对象上一次使用的时间，交由PoolManager进行自动销毁算法判断
        public bool Active { get; private set; } // 是否激活中，OnActivate()和OnDeactivate()会进行修改

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
        }

        public void OnIdleDestroy() // PoolManager自动销毁对象时
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
        public void Init()
        {
            Model = new BulletModel();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 开火时，设置子弹位置角度与速度
        /// </summary>
        public void Fire(Vector2 direction)
        {
            var thisTransform = transform;
            thisTransform.position = position;
            thisTransform.eulerAngles = new Vector3(0, 0, Model.RotationZ);
            bulletRigidbody2D.velocity = direction * Model.BulletSpeed;
            gameObject.SetActive(true);

            if (Model.BulletExitTime > 0)
            {
                StartCoroutine(BulletReturnByTime());
            }
        }

        /// <summary>
        /// 子弹到达时间上限自动回池    //todo:有问题
        /// </summary>
        /// <returns></returns>
        private IEnumerator BulletReturnByTime()
        {
            yield return new WaitForSeconds(Model.BulletExitTime);
            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }
        }

        /// <summary>
        /// 检测是否射到物体
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Model.BulletType == BulletType.Player)
            {
                //玩家的子弹
                if (!other.gameObject.tag.Equals("Enemy"))
                {
                    return;
                }

                var enemyCtrl = BattleManager.BattleManager.Instance.TryGetEnemyCtrl(other);
                if (!enemyCtrl)
                {
                    return; //todo:打到敌方子弹或武器，无反应
                }

                ShootEnemyEvent(enemyCtrl);
                return;
            }

            //敌人的子弹
            if (!other.gameObject.tag.Equals("Player"))
            {
                return;
            }

            var playerController = BattleManager.BattleManager.Instance.GetPlayerCollider();

            if (other == playerController)
            {
                var playerCtrl = BattleManager.BattleManager.Instance.GetPlayerCtrl();
                ShootPlayerEvent(playerCtrl); //角色受攻击
                return;
            }

            Debug.Log("打到我方子弹或武器，加钱");
            AttackEnemyBullet();
        }

        /// <summary>
        /// 子弹射击到敌人事件
        /// </summary>
        private void ShootEnemyEvent(EnemyBaseCtrl enemyController)
        {
            if (!Active)
            {
                return;
            }

            enemyController.UnderAttack(Model.Damage);

            Model.BulletPenetrationCount--;
            if (Model.BulletPenetrationCount <= 0)
            {
                PoolManager.Instance.ReturnObject(this);
            }
        }

        /// <summary>
        /// 子弹射击到玩家事件
        /// </summary>
        private void ShootPlayerEvent(PlayerController playerController)
        {
            if (!Active)
            {
                return;
            }

            playerController.UnderAttack(Model.Damage);
            PoolManager.Instance.ReturnObject(this);
        }

        /// <summary>
        /// 砍到敌人子弹加钱
        /// </summary>
        private void AttackEnemyBullet()
        {
            var pc = BattleManager.BattleManager.Instance.GetPlayerCtrl();
            Debug.Log("Money : " + pc.GetMoney());
            pc.AddMoney(Model.EnemyBulletValue);
            

            Model.BulletPenetrationCount--;
            if (Model.BulletPenetrationCount <= 0 && Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }
        }
    }
}