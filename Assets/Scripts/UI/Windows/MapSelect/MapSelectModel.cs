using UnityEngine;
using Yu;

namespace UI.Windows.MapSelect
{
    public class MapSelectModel
    {
        public GameObject LevelCellPrefab;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInit()
        {
            //加载levelcell的prefab
            var path = ConfigManager.Tables.CfgPrefab["LevelCell"].PrefabPath;
            LevelCellPrefab = AssetManager.Instance.LoadAssetGameObject(path);
        }

        /// <summary>
        /// 打开时
        /// </summary>
        public void OnOpen()
        {
        }
    }
}