using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Yu;

namespace UI.Windows.MapSelect
{
    public class MapSelectView : MonoBehaviour
    {
        public TMP_InputField levelID;
        public Button gameStart;
        public Button gameReturn;

        public Transform scrollWindowTransform;
        
        public ScrollRect scrollRect;
        public GameObject contentObj;

        public Text textContent;
        public Text LevelName;

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