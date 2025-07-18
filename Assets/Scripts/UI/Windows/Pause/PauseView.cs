using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Windows.Pause
{
    public class PauseView : MonoBehaviour
    {
        public Button setBtn;
        public Button returnBtn;
        public Button restartBtn;
        public Button retreatBtn;

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
    }
}