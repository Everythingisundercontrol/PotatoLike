using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic.Bullet;
using GameLogic.Enemy.EnemyBase;
using GameLogic.Enemy.Enemys.BasicEnemy;
using GameLogic.Enemy.Enemys.CasterEnemy;
using GameLogic.Enemy.Enemys.DashEnemy;
using GameLogic.Enemy.Enemys.SpikerEnemy;
using GameLogic.Enemy.Enemys.TurretEnemy;
using GameLogic.Items.Box;
using GameLogic.Items.Gold;
using GameLogic.Items.HealthPack;
using GameLogic.Items.Mag;
using GameLogic.Player.MVC;
using GameLogic.Player.Weapons.Sickle;
using JetBrains.Annotations;
using UI.Windows.Battle;
using Unity.Mathematics;
using UnityEngine;
using Yu;
using Object = UnityEngine.Object;

namespace GameLogic.BattleManager
{
    public class BattleManager : BaseSingleTon<BattleManager>, IMonoManager
    {
        private BattleManagerModel _model;

        public void OnInit()
        {
            _model = new BattleManagerModel();
            _model.Init();

            _model.EnemyGenerateActions.Add("EnemyController", (Func<EnemyController>) GenerateBasicEnemy);
            _model.EnemyGenerateActions.Add("CasterEnemyCtrl", (Func<CasterEnemyCtrl>) GenerateCasterEnemy);
            _model.EnemyGenerateActions.Add("DashEnemyCtrl", (Func<DashEnemyCtrl>) GenerateDashEnemy);
            _model.EnemyGenerateActions.Add("SpikerEnemyCtrl", (Func<SpikerEnemyCtrl>) GenerateSpikerEnemy);
            _model.EnemyGenerateActions.Add("TurretEnemyCtrl", (Func<TurretEnemyCtrl>) GenerateTurretEnemy);

            EventManager.Instance.AddListener(EventName.Pause, Pause);
            EventManager.Instance.AddListener(EventName.CancelPause, CancelPause);
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
            if (_model.IfPause)
            {
                return;
            }

            CheckPlayerFixedUpdate();
            CheckEnemyFixedUpdate();
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
        }

        /// <summary>
        /// 开始进入战斗时 //todo:改，贴合地图选择
        /// </summary>
        public void OnStart()
        {
            //先获取地图的四个顶点
            GetBackGroundVertex();

            //生成玩家
            _model.PlayerGameObject = CreatePlayer();
            _model.PlayerController = _model.PlayerGameObject.GetComponent<PlayerController>();
            _model.PlayerController.Init();
            _model.PlayerCollCollider2D = _model.PlayerController.GetCollider2D();

            //绑定虚拟相机
            CameraManager.Instance.GetVCamController("VCamCharacter").RegisterFollower(_model.PlayerController.GetCamaraTarget().transform);

            //生成对象池
            PoolManager.Instance.CreatePool(1, GenerateBullet);
            PoolManager.Instance.CreatePool(1, GenerateSickleBullet);
            PoolManager.Instance.CreatePool(1, GenerateGold);
            PoolManager.Instance.CreatePool(1, GenerateBox);
            PoolManager.Instance.CreatePool(1, GenerateHealthPack);
            PoolManager.Instance.CreatePool(1, GenerateMag);

            UIManager.Instance.OpenWindow("BattleView");

            // GameManager.Instance.StartCoroutine(CycleSummonEnemy());
        }

        /// <summary>
        /// 退出战斗页面
        /// </summary>
        public void OnQuit()
        {
            //如果暂停中，就取消暂停
            if (InputManager.Instance.IfPause)
            {
                TimeScaleManager.Instance.GetTimeHolder("Game").paused = false;
                EventManager.Instance.Dispatch(EventName.CancelPause);
                InputManager.Instance.IfPause = false;
            }
            
            EventManager.Instance.RemoveListener(EventName.Pause, Pause);
            EventManager.Instance.RemoveListener(EventName.CancelPause, CancelPause);
            
            PoolManager.Instance.DestoryPool<BulletController>();
            PoolManager.Instance.DestoryPool<SickleBulletCtrl>();
            PoolManager.Instance.DestoryPool<GoldCtrl>();
            PoolManager.Instance.DestoryPool<BoxCtrl>();
            PoolManager.Instance.DestoryPool<HealthPackCtrl>();
            PoolManager.Instance.DestoryPool<MagCtrl>();
            
            _model.PlayerController.Quit();
            
            UIManager.Instance.DestroyWindow("BattleView");
            
            OnInit();
        }

        #region 测试用例

        /// <summary>
        /// 
        /// </summary>
        public void TestPlayer()
        {
            // _model.PlayerController.TestPlayerDead();

            // var box =  PoolManager.Instance.GetObject<BoxCtrl>();
            // box.transform.position = _model.GetRandomPosition();

            // var healthPack = PoolManager.Instance.GetObject<HealthPackCtrl>();
            // healthPack.transform.position = _model.GetRandomPosition();

            var mag = PoolManager.Instance.GetObject<MagCtrl>();
            mag.transform.position = _model.GetRandomPosition();
        }

        /// <summary>
        /// 武器切换
        /// </summary>
        public void TestWeaponsChange()
        {
            _model.PlayerController.TestWeaponsChange();
        }

        /// <summary>
        /// 武器大小变化
        /// </summary>
        /// <param name="num"></param>
        public void TestChangeWeaponScale(float num)
        {
            _model.PlayerController.TestChangeWeaponScale(num);
        }

        /// <summary>
        /// 测试敌人生成
        /// </summary>
        public void TestEnemy()
        {
            EnemySummoner<CasterEnemyCtrl>();
            EnemySummoner<DashEnemyCtrl>();
            EnemySummoner<SpikerEnemyCtrl>();
            EnemySummoner<TurretEnemyCtrl>();
            EnemySummoner<EnemyController>();
        }

        #endregion

        /// <summary>
        /// 获得敌人列表
        /// </summary>
        /// <returns></returns>
        public List<EnemyBaseCtrl> GetEnemyBaseCtrl()
        {
            return _model.EnemyBaseControllers;
        }

        /// <summary>
        /// 生成敌人
        /// </summary>
        public void EnemySummoner<T>() where T : EnemyBaseCtrl
        {
            if (!PoolManager.Instance.CheckPoolExit<T>()) //如果不存在该池，则新建一个，从字典里查
            {
                PoolManager.Instance.CreatePool(1, (Func<T>) _model.EnemyGenerateActions[typeof(T).Name]);
            }

            var ec = PoolManager.Instance.GetObject<T>();
            ec.gameObject.transform.position = _model.GetRandomPosition();
            _model.EnemyBaseControllers.Add(ec);
            _model.Collider2EnemyCtrl.Add(ec.GetCollider2D(), ec);
            _model.SetSummonInterval();
        }

        /// <summary>
        /// 从ec池中移除enemyController
        /// </summary>
        /// <param name="enemyBaseCtrl"></param>
        public void RemoveEnemyControllerFromList(EnemyBaseCtrl enemyBaseCtrl)
        {
            if (!_model.EnemyBaseControllers.Contains(enemyBaseCtrl))
            {
                return;
            }

            _model.EnemyBaseControllers.Remove(enemyBaseCtrl);
            _model.Collider2EnemyCtrl.Remove(enemyBaseCtrl.GetCollider2D());
            UIManager.Instance.GetCtrl<BattleCtrl>("BattleView").EnemyHpCtrlReturnPool(enemyBaseCtrl);
        }

        /// <summary>
        /// 获取玩家位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPlayerPosition()
        {
            return _model.PlayerController.transform.position;
        }

        /// <summary>
        /// 获取玩家Ctrl
        /// </summary>
        /// <returns></returns>
        public PlayerController GetPlayerCtrl()
        {
            return _model.PlayerController;
        }

        /// <summary>
        /// 获取玩家Collider2D
        /// </summary>
        /// <returns></returns>
        public Collider2D GetPlayerCollider()
        {
            return _model.PlayerCollCollider2D;
        }

        /// <summary>
        /// 获取EnemyCtrl
        /// </summary>
        /// <param name="collider2D"></param>
        /// <returns></returns>
        [CanBeNull]
        public EnemyBaseCtrl TryGetEnemyCtrl(Collider2D collider2D)
        {
            return _model.Collider2EnemyCtrl.ContainsKey(collider2D) ? _model.Collider2EnemyCtrl[collider2D] : null;
        }

        /// <summary>
        /// Player的FixedUpdate
        /// </summary>
        private void CheckPlayerFixedUpdate()
        {
            if (!_model.PlayerController)
            {
                return;
            }

            _model.PlayerController.PlayerFixedUpdate();
        }

        /// <summary>
        /// EnemyList的FixedUpdate
        /// </summary>
        private void CheckEnemyFixedUpdate()
        {
            if (_model.EnemyBaseControllers.Count <= 0)
            {
                return;
            }

            foreach (var enemyBaseController in _model.EnemyBaseControllers)
            {
                if (_model.PlayerController)
                {
                    enemyBaseController.EnemyUpdate(_model.PlayerController.transform.position);
                }
            }
        }

        /// <summary>
        /// 生成玩家
        /// </summary>
        private static GameObject CreatePlayer()
        {
            var path = ConfigManager.Tables.CfgPrefab["Player 0"].PrefabPath;

            var prefab = AssetManager.Instance.LoadAsset<GameObject>(path);

            return Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// 生成金币
        /// </summary>
        /// <returns></returns>
        private GoldCtrl GenerateGold()
        {
            var path = ConfigManager.Tables.CfgPrefab["Gold"].PrefabPath;
            var goldPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var gold = Object.Instantiate(goldPrefab, Vector3.zero, quaternion.identity);
            var gc = gold.GetComponent<GoldCtrl>();
            gc.GoldInit();

            return gc;
        }

        /// <summary>
        /// 生成箱子
        /// </summary>
        /// <returns></returns>
        private BoxCtrl GenerateBox()
        {
            var path = ConfigManager.Tables.CfgPrefab["Box"].PrefabPath;
            var boxPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var box = Object.Instantiate(boxPrefab, Vector3.zero, quaternion.identity);
            var bc = box.GetComponent<BoxCtrl>();
            bc.BoxInit();

            return bc;
        }

        /// <summary>
        /// 生成治疗包
        /// </summary>
        /// <returns></returns>
        private HealthPackCtrl GenerateHealthPack()
        {
            var path = ConfigManager.Tables.CfgPrefab["HealthPack"].PrefabPath;
            var healthPackPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var healthPack = Object.Instantiate(healthPackPrefab, Vector3.zero, quaternion.identity);
            var hc = healthPack.GetComponent<HealthPackCtrl>();
            hc.Init();

            return hc;
        }

        /// <summary>
        /// 生成磁铁
        /// </summary>
        /// <returns></returns>
        private MagCtrl GenerateMag()
        {
            var path = ConfigManager.Tables.CfgPrefab["Mag"].PrefabPath;
            var magPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var mag = Object.Instantiate(magPrefab, Vector3.zero, quaternion.identity);
            var mc = mag.GetComponent<MagCtrl>();
            mc.Init();

            return mc;
        }

        /// <summary>
        /// 对象池生成敌人
        /// </summary>
        /// <returns></returns>
        private EnemyController GenerateBasicEnemy()
        {
            var path = ConfigManager.Tables.CfgPrefab["BasicEnemy"].PrefabPath;
            var enemyPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var enemy = Object.Instantiate(enemyPrefab, Vector3.zero, quaternion.identity);
            var ec = enemy.GetComponent<EnemyController>();
            ec.EnemyInit();

            return ec;
        }

        /// <summary>
        /// 对象池生成敌人
        /// </summary>
        /// <returns></returns>
        private CasterEnemyCtrl GenerateCasterEnemy()
        {
            var path = ConfigManager.Tables.CfgPrefab["CasterEnemy"].PrefabPath;
            var enemyPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var enemy = Object.Instantiate(enemyPrefab, Vector3.zero, quaternion.identity);
            var ec = enemy.GetComponent<CasterEnemyCtrl>();
            ec.EnemyInit();

            return ec;
        }

        /// <summary>
        /// 对象池生成敌人
        /// </summary>
        /// <returns></returns>
        private DashEnemyCtrl GenerateDashEnemy()
        {
            var path = ConfigManager.Tables.CfgPrefab["DashEnemy"].PrefabPath;
            var enemyPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var enemy = Object.Instantiate(enemyPrefab, Vector3.zero, quaternion.identity);
            var ec = enemy.GetComponent<DashEnemyCtrl>();
            ec.EnemyInit();

            return ec;
        }

        /// <summary>
        /// 对象池生成敌人
        /// </summary>
        /// <returns></returns>
        private SpikerEnemyCtrl GenerateSpikerEnemy()
        {
            var path = ConfigManager.Tables.CfgPrefab["SpikerEnemy"].PrefabPath;
            var enemyPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var enemy = Object.Instantiate(enemyPrefab, Vector3.zero, quaternion.identity);
            var ec = enemy.GetComponent<SpikerEnemyCtrl>();
            ec.EnemyInit();

            return ec;
        }

        /// <summary>
        /// 对象池生成敌人
        /// </summary>
        /// <returns></returns>
        private TurretEnemyCtrl GenerateTurretEnemy()
        {
            var path = ConfigManager.Tables.CfgPrefab["TurretEnemy"].PrefabPath;
            var enemyPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var enemy = Object.Instantiate(enemyPrefab, Vector3.zero, quaternion.identity);
            var ec = enemy.GetComponent<TurretEnemyCtrl>();
            ec.EnemyInit();

            return ec;
        }

        /// <summary>
        /// 生成子弹并返回子弹controller
        /// </summary>
        /// <returns></returns>
        private BulletController GenerateBullet()
        {
            var path = ConfigManager.Tables.CfgPrefab["Bullet 3"].PrefabPath;
            var buPrefab = AssetManager.Instance.LoadAssetGameObject(path);

            var bullet = Object.Instantiate(buPrefab, Vector3.zero, quaternion.identity);
            var bulletCtrl = bullet.GetComponent<BulletController>();
            bulletCtrl.Init();

            return bulletCtrl;
        }

        /// <summary>
        /// 生成镰刀子弹并返回子弹Ctrl
        /// </summary>
        /// <returns></returns>
        private SickleBulletCtrl GenerateSickleBullet()
        {
            var path = ConfigManager.Tables.CfgPrefab["SickleBulletPrefab"].PrefabPath;
            var buPrefab = AssetManager.Instance.LoadAssetGameObject(path);

            var bullet = Object.Instantiate(buPrefab, Vector3.zero, quaternion.identity);
            var sickleBulletCtrl = bullet.GetComponent<SickleBulletCtrl>();
            sickleBulletCtrl.Init();

            return sickleBulletCtrl;
        }

        /// <summary>
        /// 获取地图顶点
        /// </summary>
        private void GetBackGroundVertex()
        {
            _model.Po1 = GameObject.Find("Point01").transform.position;
            _model.Po3 = GameObject.Find("Point03").transform.position;
        }

        /// <summary>
        /// 协程循环召唤敌人    //todo：
        /// </summary>
        /// <returns></returns>
        private IEnumerator CycleSummonEnemy()
        {
            yield return new WaitForSeconds(_model.SummonInterval);
            // EnemySummoner();
            // StartCoroutine(CycleSummonEnemy());
        }

        /// <summary>
        /// 暂停
        /// </summary>
        private void Pause()
        {
            _model.IfPause = true;
        }

        /// <summary>
        /// 取消暂停
        /// </summary>
        private void CancelPause()
        {
            _model.IfPause = false;
        }
    }
}