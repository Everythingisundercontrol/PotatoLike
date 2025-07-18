using System.Collections.Generic;
using GameLogic.Player.WeaponBase;
using UI.Windows.Battle;
using UnityEditor.Rendering;
using UnityEngine;
using Yu;
using Random = UnityEngine.Random;

namespace GameLogic.Player.MVC
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerModel _model;
        private PlayerView _view;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            _model = new PlayerModel();
            _view = GetComponent<PlayerView>();
            _model.Init();

            SetWeaponsCtrl();
            EventBind();
        }

        public void Quit()
        {
            EventManager.Instance.RemoveListener<bool>(EventName.RunAndStopAnimChange, _view.SetIsRunAnim);
            EventManager.Instance.RemoveListener(EventName.Heal, Heal);
        }

        /// <summary>
        /// 帧调用
        /// </summary>
        public void PlayerFixedUpdate()
        {
            Move();
            WeaponsFixUpdate();
        }

        #region test

        /// <summary>
        /// 
        /// </summary>
        public void TestPlayerDead()
        {
            _model.IsDead = !_model.IsDead;
        }

        public void TestWeaponsChange()
        {
            ChangeWeaponsRandom();
        }

        public void TestChangeWeaponScale(float num)
        {
            foreach (var weaponCtrlBase in _model.WeaponCons)
            {
                weaponCtrlBase.SetScaleNum(num);
            }
        }

        #endregion


        /// <summary>
        /// 被攻击
        /// </summary>
        public void UnderAttack(float enemyAtk)
        {
            if (!_model.IsUnderAttackAnim)
            {
                _model.IsUnderAttackAnim = true;
                StartCoroutine(_view.UnderAttackAnim());
                _model.IsUnderAttackAnim = false;
            }

            var atk = _model.AttackedArmorCheck(enemyAtk);
            if (atk <= 0)
            {
                return;
            }

            _model.Hp -= (int) atk;

            HpCheck();

            _model.LastUnderAttackTime = Time.time;

            UIManager.Instance.GetCtrl<BattleCtrl>("BattleView").SetHp(_model.Hp, _model.maxHp);
        }

        /// <summary>
        /// 获取伤害值   //todo：改进，需要伤害复杂计算，而且是实时的而不是射出去时的
        /// </summary>
        /// <returns></returns>
        public float GetDamage()
        {
            return _model.Attack;
        }

        /// <summary>
        /// 获取Collider2D
        /// </summary>
        /// <returns></returns>
        public Collider2D GetCollider2D()
        {
            return _view.playerCollider2D;
        }

        /// <summary>
        /// 获取相机跟随目标
        /// </summary>
        /// <returns></returns>
        public GameObject GetCamaraTarget()
        {
            return _view.cameraTarget;
        }

        /// <summary>
        /// 加钱
        /// </summary>
        /// <param name="addNum"></param>
        public void AddMoney(int addNum)
        {
            _model.Money += addNum;
            UIManager.Instance.GetCtrl<BattleCtrl>("BattleView").SetGold(_model.Money);
        }

        /// <summary>
        /// 加盒子
        /// </summary>
        public void AddBox()
        {
            _model.BoxNum++;
            UIManager.Instance.GetCtrl<BattleCtrl>("BattleView").ChangeBoxNum(_model.BoxNum);
        }

        /// <summary>
        /// 盒子数归零
        /// </summary>
        public void BoxReset()
        {
            _model.BoxNum = 0;
        }

        /// <summary>
        /// 加血包数
        /// </summary>
        public void AddHealthPack()
        {
            _model.HealthPackNum++;
            UIManager.Instance.GetCtrl<BattleCtrl>("BattleView").ChangeHealthPackNum(_model.HealthPackNum);
        }

        /// <summary>
        /// 获取当前多少钱
        /// </summary>
        /// <returns></returns>
        public int GetMoney()
        {
            return _model.Money;
        }

        public int GetHp()
        {
            return _model.Hp;
        }

        public int GetHpMax()
        {
            return _model.maxHp;
        }

        /// <summary>
        /// 血条检测
        /// </summary>
        private void HpCheck()
        {
            if (_model.IsDead)
            {
                return;
            }

            if (_model.Hp <= 0)
            {
                //死亡
                Dead();
            }
        }

        /// <summary>
        /// 角色死亡处理
        /// </summary>
        private void Dead()
        {
            _model.IsDead = true;
            _view.SetIsDeadAnim(true);
            _model.MovementSpeed += 3f;
        }

        /// <summary>
        /// 帧移动集合
        /// </summary>
        private void Move()
        {
            InputMovement();
            InputCheck();
            OnMove();
            MoveAndStopCheck();
        }

        /// <summary>
        /// 移动值输入
        /// </summary>
        private void InputMovement()
        {
            if (InputManager.Instance.MovementPressed)
            {
                SetMovement(InputManager.Instance.CurrentMovement);
                FaceCheck();
                return;
            }

            SetMovement(Vector2.zero);
        }

        /// <summary>
        /// 设置移动值。
        /// </summary>
        /// <param name="value">-1~0为左，0~1为右 </param>
        private void SetMovement(Vector2 value)
        {
            _model.HorizontalMovement = value.x;
            _model.VerticalMovement = value.y;
        }

        /// <summary>
        /// 事件绑定
        /// </summary>
        private void EventBind()
        {
            EventManager.Instance.AddListener<bool>(EventName.RunAndStopAnimChange, _view.SetIsRunAnim);
            EventManager.Instance.AddListener(EventName.Heal, Heal);
        }

        /// <summary>
        /// 玩家回血
        /// </summary>
        private void Heal()
        {
            if (_model.HealthPackNum <= 0)
            {
                return;
            }

            _model.HealthPackNum--;
            if (_model.Hp + 10 <= _model.maxHp)
            {
                _model.Hp += 10;
            }

            if (_model.Hp + 10 > _model.maxHp)
            {
                _model.Hp = _model.maxHp;
            }

            UIManager.Instance.GetCtrl<BattleCtrl>("BattleView").SetHp(_model.Hp, _model.maxHp);
            UIManager.Instance.GetCtrl<BattleCtrl>("BattleView").ChangeHealthPackNum(_model.HealthPackNum);
        }

        /// <summary>
        /// 先判断是否有键入
        /// </summary>
        private void InputCheck()
        {
            if (InputManager.Instance.MovementPressed)
            {
                _model.OnInputSpeedCheck();
                return;
            }

            _model.NotInputSpeedCheck();
        }

        /// <summary>
        /// 移动
        /// </summary>
        private void OnMove()
        {
            _view.playerRigidbody2D.velocity = _model.Speed;
        }

        /// <summary>
        /// 移动/站立动画检查
        /// </summary>
        private void MoveAndStopCheck()
        {
            if (_model.Speed.x == 0 && _model.Speed.y == 0)
            {
                _model.IsRun = false;
                return;
            }

            _model.IsRun = true;
        }

        /// <summary>
        /// 决定人物朝向
        /// </summary>
        private void FaceCheck()
        {
            //按右方向键
            if (_model.FaceToLeft && InputManager.Instance.CurrentMovement.x > 0)
            {
                _model.FaceToLeft = false;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            //按左方向键
            if (_model.FaceToLeft || !(InputManager.Instance.CurrentMovement.x < 0))
            {
                return;
            }

            _model.FaceToLeft = true;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        #region 武器

        private void ForeachWeaponsQuit()
        {
            if (_model.WeaponCons.Count <= 0)
            {
                return;
            }

            foreach (var weaponCtrl in _model.WeaponCons)
            {
                weaponCtrl.Quit();
                Destroy(weaponCtrl.gameObject);
            }

            _model.WeaponCons.Clear();
        }

        /// <summary>
        /// 武器固定帧调用
        /// </summary>
        private void WeaponsFixUpdate()
        {
            if (_model.IsDead)
            {
                return;
            }

            if (_model.WeaponCons.Count <= 0)
            {
                return;
            }

            foreach (var weaponCtrl in _model.WeaponCons)
            {
                weaponCtrl.WeaponFixedUpdate();
            }
        }

        /// <summary>
        /// 武器初始化
        /// </summary>
        private void WeaponInit()
        {
            if (_model.WeaponCons.Count <= 0)
            {
                return;
            }

            foreach (var weaponCtrl in _model.WeaponCons)
            {
                weaponCtrl.Init();
            }
        }

        /// <summary>
        /// 设置武器Ctrl
        /// </summary>
        private void SetWeaponsCtrl()
        {
            _model.WeaponCons = new List<WeaponCtrlBase>();
            var path = ConfigManager.Tables.CfgPrefab[_model.WeaponsList[0]].PrefabPath;
            CreateWeapons(path);
            WeaponInit();
        }

        /// <summary>
        /// 创建武器
        /// </summary>
        /// <param name="path"></param>
        private void CreateWeapons(string path)
        {
            var prefab = AssetManager.Instance.LoadAsset<GameObject>(path);
            var weaponLeft = Instantiate(prefab, _view.handLeftObj.transform);
            var weaponRight = Instantiate(prefab, _view.handRightObj.transform);
            _model.WeaponCons.Add(weaponLeft.GetComponent<WeaponCtrlBase>());
            _model.WeaponCons.Add(weaponRight.GetComponent<WeaponCtrlBase>());
        }

        /// <summary>
        /// 改变武器（随机）
        /// </summary>
        private void ChangeWeaponsRandom()
        {
            ForeachWeaponsQuit();
            var randomNum = Random.Range(0, _model.WeaponsList.Count);
            var path = ConfigManager.Tables.CfgPrefab[_model.WeaponsList[randomNum]].PrefabPath;
            CreateWeapons(path);
            WeaponInit();
            UIManager.Instance.GetCtrl<BattleCtrl>("BattleView").ChangeWeaponImage(randomNum);
        }

        /// <summary>
        /// 改变武器（通过名字定向）
        /// </summary>
        /// <param name="weaponName"></param>
        private void ChangeWeaponsByName(string weaponName)
        {
            if (!_model.WeaponsList.Contains(weaponName))
            {
                Debug.LogError(weaponName + "在WeaponsList中不存在！");
                return;
            }

            ForeachWeaponsQuit();
            var path = ConfigManager.Tables.CfgPrefab[weaponName].PrefabPath;
            CreateWeapons(path);
            WeaponInit();
        }

        #endregion

        /// <summary>
        /// 敌人碰撞到我方单位
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.tag.Equals("Enemy"))
            {
                return;
            }

            if (!UnderAttackAbleCheck())
            {
                return;
            }

            var enemyCtrl = BattleManager.BattleManager.Instance.TryGetEnemyCtrl(other.collider);
            if (!enemyCtrl)
            {
                return;
            }

            UnderAttack(enemyCtrl.GetAttack());
        }

        /// <summary>
        /// 敌人持续触碰到我方单位
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.tag.Equals("Enemy"))
            {
                return;
            }

            if (!UnderAttackAbleCheck())
            {
                return;
            }

            var enemyCtrl = BattleManager.BattleManager.Instance.TryGetEnemyCtrl(other.collider);
            if (!enemyCtrl)
            {
                return;
            }

            UnderAttack(enemyCtrl.GetAttack());
            Debug.Log("OnCollisionStay " + other.gameObject.name + _model.Hp);
        }

        /// <summary>
        /// 能否受攻击检查
        /// </summary>
        private bool UnderAttackAbleCheck()
        {
            return Time.time - _model.LastUnderAttackTime > _model.UnderAttackCoolDown;
        }
    }
}