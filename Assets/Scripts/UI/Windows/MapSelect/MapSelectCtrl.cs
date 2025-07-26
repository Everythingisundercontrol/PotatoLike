using System;
using System.Collections.Generic;
using GameLogic.Items.Gold;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Yu;

namespace UI.Windows.MapSelect
{
    public class MapSelectCtrl : UICtrlBase
    {
        private MapSelectModel _model;
        private MapSelectView _view;

        private LinkedList<LevelCellCtrl> _levelCellCtrlList; //需要维护的激活中的levelCell

        public void FixedUpdate()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            // if (_view.scrollRect.velocity.magnitude <= 0.1f)
            // {
            //     return;
            // }

            foreach (var levelCellCtrl in _levelCellCtrlList)
            {
                levelCellCtrl.ChangeScale();
            }

            CellCheck();
        }

        public override void OnInit(params object[] param)
        {
            _model = new MapSelectModel();
            _view = GetComponent<MapSelectView>();
            _model.OnInit();
            PoolManager.Instance.CreatePool(10, GenerateLevelCell); //初始化时创建对象池
            _levelCellCtrlList = new LinkedList<LevelCellCtrl>();
            for (var i = 0; i < 5; i++)
            {
                SetLevelCellOnFirst();
                SetLevelCellOnLast();
            }
        }

        public override void OpenRoot(params object[] param)
        {
            _model.OnOpen();
            _view.OpenWindow();
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
        }

        public override void OnClear()
        {
        }

        public override void BindEvent()
        {
            _view.gameStart.onClick.AddListener(GameStartOnClick);
            _view.gameReturn.onClick.AddListener(GameReturnOnClick);
            // _view.levelID.onDeselect.AddListener();
        }

        /// <summary>
        /// 检测头尾指针是否应该回收，是否应该插入。
        /// </summary>
        private void CellCheck()
        {
            //检测头尾指针是否应该回收，是否应该插入。
            var position = _view.scrollWindowTransform.position;

            var firstDis = Mathf.Abs(_levelCellCtrlList.First.Value.transform.position.x - position.x);
            var lastDis = Mathf.Abs(_levelCellCtrlList.Last.Value.transform.position.x - position.x);

            // Debug.Log(firstDis + "   " + lastDis);

            switch (firstDis)
            {
                case >= 1800:
                    RemoveLevelCellOnFirst();
                    break;
                case <= 1200:
                    SetLevelCellOnFirst();
                    break;
            }

            switch (lastDis)
            {
                case >= 1800:
                    RemoveLevelCellOnLast();
                    break;
                case <= 1200:
                    SetLevelCellOnLast();
                    break;
            }
        }

        /// <summary>
        /// 点击确认按钮开始游戏,获取游戏输入框的内容
        /// </summary>
        private void GameStartOnClick()
        {
            if (string.IsNullOrEmpty(_view.levelID.text))
            {
                Debug.Log("_view.levelID.text IsNullOrEmpty");
                return;
            }

            GameManager.GoToBattle(_view.levelID.text);
        }

        /// <summary>
        /// 点击返回按钮返回Home界面
        /// </summary>
        private void GameReturnOnClick()
        {
            CloseRoot();
            UIManager.Instance.OpenWindow("HomeView");
        }

        /// <summary>
        /// 关卡输入框被点击
        /// </summary>
        private void LevelIDOnClick()
        {
        }

        /// <summary>
        /// 关卡输入框取消选择
        /// </summary>
        private void LevelIDOnDeselect()
        {
        }

        /// <summary>
        /// 生成关卡cell
        /// </summary>
        private LevelCellCtrl GenerateLevelCell() //创建时默认不激活
        {
            var levelCell = Instantiate(_model.LevelCellPrefab, _view.contentObj.transform);
            var levelCellCtrl = levelCell.GetComponent<LevelCellCtrl>();
            levelCellCtrl.OnInit(_view.scrollWindowTransform);

            // _levelCellCtrlList.Add(levelCellCtrl);

            return levelCellCtrl;
        }

        /// <summary>
        /// 设置levelCell在第一个
        /// </summary>
        private void SetLevelCellOnFirst()
        {
            var ctrl = PoolManager.Instance.GetObject<LevelCellCtrl>();
            var ctrlGameObject = ctrl.gameObject;

            _levelCellCtrlList.AddFirst(ctrl);
            ctrlGameObject.transform.SetAsFirstSibling(); // 插入到Content的第一个位置
            var pos = _view.contentObj.transform.position;
            _view.contentObj.transform.position = new Vector3(pos.x - 600, pos.y, pos.z);
        }

        /// <summary>
        /// 设置levelCell在content的最后一个
        /// </summary>
        private void SetLevelCellOnLast()
        {
            var ctrl = PoolManager.Instance.GetObject<LevelCellCtrl>();
            var ctrlGameObject = ctrl.gameObject;

            _levelCellCtrlList.AddLast(ctrl);
            ctrlGameObject.transform.SetAsLastSibling(); // 插入到Content的最后一个位置
            var pos = _view.contentObj.transform.position;
            _view.contentObj.transform.position = new Vector3(pos.x + 600, pos.y, pos.z);
        }

        /// <summary>
        /// 移除第一个
        /// </summary>
        private void RemoveLevelCellOnFirst()
        {
            _levelCellCtrlList.First.Value.ReturnToPool();
            _levelCellCtrlList.RemoveFirst();
        }

        /// <summary>
        /// 移除最后一个
        /// </summary>
        private void RemoveLevelCellOnLast()
        {
            _levelCellCtrlList.Last.Value.ReturnToPool();
            _levelCellCtrlList.RemoveLast();
        }
    }
}