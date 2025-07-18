// ******************************************************************
//@file         HomeCtrl.cs
//@brief        主界面controller
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.22 20:38:41
// ******************************************************************

using Yu;

public class HomeCtrl : UICtrlBase
{
    private HomeModel _model;
    private HomeView _view;

    public override void OnInit(params object[] param)
    {
        _model = new HomeModel();
        _view = GetComponent<HomeView>();
        _model.OnInit();
    }

    public override void OpenRoot(params object[] param)
    {
        //EventManager.Instance.AddListener(EventName.OnLastSaveGameChange, OnLastSaveGameChange);
        _model.OnOpen();
        _view.OpenWindow();
    }

    public override void CloseRoot()
    {
        _view.CloseWindow();
        //EventManager.Instance.RemoveListener(EventName.OnLastSaveGameChange, OnLastSaveGameChange);
    }

    public override void OnClear()
    {
    }

    public override void BindEvent()
    {
        _view.btnNewGame.onClick.AddListener(BtnOnClickNewGame);
        _view.btnSetting.onClick.AddListener(BtnOnClickSetting);
        _view.btnQuit.onClick.AddListener(GameManager.QuitApplication);
    }

    /// <summary>
    /// 新游戏
    /// </summary>
    private void BtnOnClickNewGame()
    {
        CloseRoot();
        UIManager.Instance.OpenWindow("MapSelectView");//进入关卡选择界面
    }

    /// <summary>
    /// 设置
    /// </summary>
    private static void BtnOnClickSetting()
    {
        // UIManager.Instance.OpenWindow("SettingView");
    }
    
    // /// <summary>
    // /// 读取存档
    // /// </summary>
    // private static void BtnOnClickLoad()
    // {
    //     UIManager.Instance.OpenWindow("LoadGameView");
    // }
    //
    // /// <summary>
    // /// 继续游戏
    // /// </summary>
    // private void BtnOnClickContinue()
    // {
    //     var saveGameIndex = SaveManager.Get(GlobalDef.LastSaveGameIndex, -1, SaveType.Global);
    //     if (saveGameIndex < 0)
    //     {
    //         Debug.LogError("存档序号错误");
    //         _view.RefreshLastSaveGame();
    //         return;
    //     }
    //
    //     //新建存档
    //     if (!SaveManager.ExistFile(SaveManager.GetSaveGameFileName(saveGameIndex)))
    //     {
    //         GameManager.Instance.CreatSaveGame(saveGameIndex);
    //         GameManager.Instance.LoadSaveGame(saveGameIndex);
    //         return;
    //     }
    //
    //     GameManager.Instance.LoadSaveGame(saveGameIndex);
    // }

    // /// <summary>
    // /// 当前最新存档发生变化时
    // /// </summary>
    // private void OnLastSaveGameChange()
    // {
    //     _view.RefreshLastSaveGame();
    // }
}