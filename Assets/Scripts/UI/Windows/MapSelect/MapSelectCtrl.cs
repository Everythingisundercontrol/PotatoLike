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


        public override void OnInit(params object[] param)
        {
            _model = new MapSelectModel();
            _view = GetComponent<MapSelectView>();
            _model.OnInit();
            PoolManager.Instance.CreatePool(1, GenerateLevelCell);
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
        /// 点击确认按钮开始游戏,获取游戏输入框的内容
        /// </summary>
        private void GameStartOnClick()
        {
            if (string.IsNullOrEmpty(_view.levelID.text))
            {
                Debug.Log("_view.levelID.text IsNullOrEmpty");
                return;
            }

            //todo:加判断，是否异常
            // Debug.Log(_view.levelID.text + "" + _view.levelID.text.GetType());
            GameManager.GoToBattle(_view.levelID.text);
            // GameManager.GoToBattle("SampleScene");
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
        private LevelCellCtrl GenerateLevelCell()
        {
            var levelCell = Instantiate(_model.LevelCellPrefab, _view.contentObj.transform);
            var levelCellCtrl = levelCell.GetComponent<LevelCellCtrl>();
            levelCellCtrl.OnInit();

            
            return levelCellCtrl;
        }
    }
}