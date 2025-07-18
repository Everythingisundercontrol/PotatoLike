using GameLogic.BattleManager;
using GameLogic.Enemy.EnemyBase;
using UI.HUD.EnemyHp;
using UnityEngine;
using Yu;

namespace UI.Windows.Battle
{
    public class BattleCtrl : UICtrlBase
    {
        private BattleModel _model;
        private BattleView _view;

        public override void OnInit(params object[] param)
        {
            _model = new BattleModel();
            _view = GetComponent<BattleView>();
            _model.OnInit();
            if (PoolManager.Instance.GetPool<EnemyHpCtrl>() != null)
            {
                PoolManager.Instance.DestoryPool<EnemyHpCtrl>();
            }

            PoolManager.Instance.CreatePool(1, GenerateEnemyHpCtrl);
        }

        public override void OpenRoot(params object[] param)
        {
            _model.OnOpen();
            _view.OpenWindow();
            var pc = BattleManager.Instance.GetPlayerCtrl();
            if (pc)
            {
                SetHp(pc.GetHp(), pc.GetHpMax());
            }

            SetTime(60);
            _model.BattlePause = false;
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
            EventManager.Instance.RemoveListener(EventName.Pause, Pause);
            EventManager.Instance.RemoveListener(EventName.CancelPause, CancelPause);
        }

        public override void OnClear()
        {
        }

        public override void BindEvent()
        {
            EventManager.Instance.AddListener(EventName.Pause, Pause);
            EventManager.Instance.AddListener(EventName.CancelPause, CancelPause);
        }

        void Update()
        {
            if (!_model.BattleEnd && !_model.BattlePause)
            {
                BattleTimeCtrl();
            }

            if (_model.EnemyBaseCtrl.Count > 0)
            {
                UpdateEnemyHp();
            }
        }

        /// <summary>
        /// 敌人血条刷新
        /// </summary>
        public void UpdateEnemyHp()
        {
            foreach (var enemyBaseCtrl in _model.EnemyBaseCtrl)
            {
                if (!_model.EnemyHp.ContainsKey(enemyBaseCtrl))
                {
                    //创建一个新的血条
                    var enemyHpCtrl = PoolManager.Instance.GetObject<EnemyHpCtrl>();
                    _model.EnemyHp.Add(enemyBaseCtrl, enemyHpCtrl);
                }

                //更新血条状态
                _model.EnemyHp[enemyBaseCtrl].ChangePosition(enemyBaseCtrl.transform.position);
                _model.EnemyHp[enemyBaseCtrl].ChangeHp(enemyBaseCtrl.GetHp(), enemyBaseCtrl.GetMaxHp());
            }
        }

        /// <summary>
        /// 敌人死亡时归还血条
        /// </summary>
        /// <param name="enemyBaseCtrl"></param>
        public void EnemyHpCtrlReturnPool(EnemyBaseCtrl enemyBaseCtrl)
        {
            PoolManager.Instance.ReturnObject(_model.EnemyHp[enemyBaseCtrl]);
            _model.EnemyHp.Remove(enemyBaseCtrl);
        }

        /// <summary>
        /// 生成敌人血条函数
        /// </summary>
        /// <returns></returns>
        public EnemyHpCtrl GenerateEnemyHpCtrl()
        {
            var path = ConfigManager.Tables.CfgPrefab["EnemyHp"].PrefabPath;
            var enemyHpPrefab = AssetManager.Instance.LoadAssetGameObject(path);
            var enemyHp = Instantiate(enemyHpPrefab, transform);
            var enemyHpCtrl = enemyHp.GetComponent<EnemyHpCtrl>();
            enemyHpCtrl.OnInit();

            return enemyHpCtrl;
        }

        /// <summary>
        /// 战斗时间控制
        /// </summary>
        public void BattleTimeCtrl()
        {
            if (_model.RemainingTime > 0)
            {
                _model.RemainingTime -= Time.deltaTime;
                _view.ChangeTime((int) _model.RemainingTime);
                return;
            }

            _model.BattleEnd = true;
            BattleEnd();
        }

        /// <summary>
        /// 战斗结束
        /// </summary>
        public void BattleEnd()
        {
            Debug.Log("BattleEnd");
        }

        /// <summary>
        /// 设置Hp血量
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="maxHp"></param>
        public void SetHp(int hp, int maxHp)
        {
            if (maxHp < hp)
            {
                return;
            }

            _view.ChangeHp(hp, maxHp);
        }

        /// <summary>
        /// 设置金钱数
        /// </summary>
        public void SetGold(int num)
        {
            _view.ChangeGold(num);
        }

        /// <summary>
        /// 设置关卡倒计时
        /// </summary>
        public void SetTime(float num)
        {
            _model.TotalTime = num;
            _model.RemainingTime = num;
        }

        /// <summary>
        /// 游戏暂停
        /// </summary>
        public void Pause()
        {
            UIManager.Instance.OpenWindow("PauseView");
            _model.BattlePause = true;
        }

        /// <summary>
        /// 取消暂停
        /// </summary>
        public void CancelPause()
        {
            UIManager.Instance.CloseWindow("PauseView");
            UIManager.Instance.OpenWindow("BattleView");
            _model.BattlePause = false;
        }

        /// <summary>
        /// 展示bossUI
        /// </summary>
        public void ShowBossUI()
        {
        }

        /// <summary>
        /// 改变武器贴图
        /// </summary>
        /// <param name="num"></param>
        public void ChangeWeaponImage(int num)
        {
            _view.ChangeWeaponImage(num);
        }

        /// <summary>
        /// 改变箱子数
        /// </summary>
        /// <param name="num"></param>
        public void ChangeBoxNum(int num)
        {
            _view.ChangeBox(num);
        }

        /// <summary>
        /// 改变血包数
        /// </summary>
        /// <param name="num"></param>
        public void ChangeHealthPackNum(int num)
        {
            _view.ChangeHealthPack(num);
        }
    }
}