using Yu;

namespace UI.Windows.BattleEnd
{
    public class BattleEndCtrl: UICtrlBase
    {
        private BattleEndModel _model;
        private BattleEndView _view;


        public override void OnInit(params object[] param)
        {
            _model = new BattleEndModel();
            _view = GetComponent<BattleEndView>();
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
        }
    }
}