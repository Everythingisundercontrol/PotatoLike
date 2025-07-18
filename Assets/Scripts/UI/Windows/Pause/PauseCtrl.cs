using GameLogic.BattleManager;
using Yu;

namespace UI.Windows.Pause
{
    public class PauseCtrl : UICtrlBase
    {
        private PauseModel _model;
        private PauseView _view;


        public override void OnInit(params object[] param)
        {
            _model = new PauseModel();
            _view = GetComponent<PauseView>();
            _model.OnInit();
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
            _view.setBtn.onClick.AddListener(SetBtnOnClick);
            _view.returnBtn.onClick.AddListener(ReturnBtnOnClick);
            _view.restartBtn.onClick.AddListener(RestartBtnOnClick);
            _view.retreatBtn.onClick.AddListener(RetreatBtnOnClick);
        }

        /// <summary>
        /// 设置按钮点击事件
        /// </summary>
        private void SetBtnOnClick()
        {
            //todo:没做
        }
        
        /// <summary>
        /// 返回按钮点击事件
        /// </summary>
        private void ReturnBtnOnClick()
        {
            TimeScaleManager.Instance.GetTimeHolder("Game").paused = false;
            EventManager.Instance.Dispatch(EventName.CancelPause);
            InputManager.Instance.IfPause = false;
            CloseRoot();
            UIManager.Instance.OpenWindow("BattleView");
        }
        
        /// <summary>
        /// 重开按钮点击事件
        /// </summary>
        private void RestartBtnOnClick()
        {
            BattleManager.Instance.OnQuit();
            // GameManager.ReturnToBattle();//todo:获取当前地图的sceneName
        }
        
        /// <summary>
        /// 撤退按钮点击事件
        /// </summary>
        private void RetreatBtnOnClick()
        {
            BattleManager.Instance.OnQuit();
            GameManager.GoToTitle("HomeView");
        }
        
    }
}