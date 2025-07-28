using System.Collections.Generic;
using UnityEngine;
using Yu;

namespace UI.Windows.MapSelect
{
    public class MapSelectModel
    {
        public GameObject LevelCellPrefab;
        public List<string> LevelID;
        public string FocusLevelID;

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInit()
        {
            //加载levelcell的prefab
            var path = ConfigManager.Tables.CfgPrefab["LevelCell"].PrefabPath;
            LevelCellPrefab = AssetManager.Instance.LoadAssetGameObject(path);

            LevelID = new List<string>();
            foreach (var rowCfgScene in ConfigManager.Tables.CfgScene.DataList)
            {
                if (!string.IsNullOrEmpty(rowCfgScene.LevelName))
                {
                    LevelID.Add(rowCfgScene.Id);
                }
            }
        }

        /// <summary>
        /// 打开时
        /// </summary>
        public void OnOpen()
        {
        }
    }
}