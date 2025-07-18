using UnityEngine;

namespace UI.Windows.BattleEnd
{
    public class BattleEndView : MonoBehaviour
    {
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