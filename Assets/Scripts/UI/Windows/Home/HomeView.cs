// ******************************************************************
//@file         HomeView.cs
//@brief        主界面View
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.22 20:38:27
// ******************************************************************

using UnityEngine;
using UnityEngine.UI;

public class HomeView : MonoBehaviour
{
    public Button btnNewGame;
    public Button btnSetting;
    public Button btnQuit;
    
    /// <summary>
    /// 打开窗口
    /// </summary>
    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    public void CloseWindow()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        gameObject.SetActive(false);
    }

    // /// <summary>
    // /// 刷新最新存档摁钮
    // /// </summary>
    // public void RefreshLastSaveGame()
    // {
    //     var lastSaveGame = SaveManager.Get(GlobalDef.LastSaveGameIndex, -1, SaveType.Global);
    //     var hasLastSaveGame = lastSaveGame >= 0;
    //     btnContinue.gameObject.SetActive(hasLastSaveGame);
    // }
}