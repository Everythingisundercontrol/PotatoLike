// ******************************************************************
//@file         UIManager.cs
//@brief        C#层UI系统
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:31:09
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using XLua;

namespace Yu
{
    public class UIManager : BaseSingleTon<UIManager>, IMonoManager
    {
        private CfgUI _cfgUI;
        private Transform _uiRoot;
        private Dictionary<string, Transform> _layers;
        private Dictionary<string, UICtrlBase> _allViews;
        private Dictionary<string, Stack<UICtrlBase>> _layerStacks;
        private readonly List<UICtrlBase> _cachePopViewCtrl = new List<UICtrlBase>(); //CloseWindowOnly使用的缓存
        private LuaTable _uiManager;


        public void OnInit()
        {
            _uiManager = LuaManager.Instance.LuaEnv.Global.Get<LuaTable>("UIManager");
            _cfgUI = ConfigManager.Tables.CfgUI;
            _uiRoot = GameObject.Find("UIRoot").transform;
            _layers = new Dictionary<string, Transform>
            {
                {"SceneLayer", GameObject.Find("SceneLayer").transform},
                {"NormalLayer", GameObject.Find("NormalLayer").transform},
                {"TopLayer", GameObject.Find("TopLayer").transform}
            };

            _layerStacks = new Dictionary<string, Stack<UICtrlBase>>
            {
                {"SceneLayer", new Stack<UICtrlBase>()},
                {"NormalLayer", new Stack<UICtrlBase>()},
                {"TopLayer", new Stack<UICtrlBase>()}
            };

            _allViews = new Dictionary<string, UICtrlBase>();

            InitHUD(); //初始化HUD
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
        }

        /// <summary>
        /// 获取uiRoot
        /// </summary>
        public Transform GetUIRoot()
        {
            return _uiRoot;
        }
        
        /// <summary>
        /// 打开页面
        /// </summary>
        public void OpenWindowByLua(string windowName)
        {
            //暂定所有界面只能打开一次
            if (CheckViewActiveInHierarchy(windowName))
            {
                return;
            }

            var funcOpenWindow = _uiManager.Get<Action<LuaTable, string>>("OpenWindow");
            funcOpenWindow?.Invoke(_uiManager, windowName);
        }
        
        /// <summary>
        /// 关闭lua界面
        /// </summary>
        public void CloseWindowByLua(string windowName)
        {
            var funcCloseWindow = _uiManager.Get<Action<LuaTable, string>>("CloseWindow");
            funcCloseWindow?.Invoke(_uiManager, windowName);
        }

        /// <summary>
        /// 打开页面
        /// </summary>
        public void OpenWindow(string windowName, params object[] param)
        {
            //暂定所有界面只能打开一次
            if (CheckViewActiveInHierarchy(windowName))
            {
                return;
            }

            var ctrl = GetCtrl<UICtrlBase>(windowName, param);
            _layerStacks[ConfigManager.Tables.CfgUI[windowName].Layer].Push(ctrl);
            ctrl.OpenRoot(param);
        }

        /// <summary>
        /// 打开页面，不添加进栈，独立控制
        /// </summary>
        public void OpenWindowWithoutStack(string windowName, params object[] param)
        {
            //暂定所有界面只能打开一次
            if (CheckViewActiveInHierarchy(windowName))
            {
                return;
            }

            var ctrl = GetCtrl<UICtrlBase>(windowName, param);
            ctrl.OpenRoot(param);
        }

        /// <summary>
        /// 关闭页面，包括关闭，与它同一Layer，且在它打开后打开的界面
        /// </summary>
        public void CloseWindow(string windowName)
        {
            var ctrl = GetCtrl<UICtrlBase>(windowName);
            var layer = ConfigManager.Tables.CfgUI[windowName].Layer;
            while (_layerStacks[layer].Count != 0)
            {
                var ctrlBefore = _layerStacks[layer].Pop();
                if (!ctrlBefore)
                {
                    Debug.LogWarning("弹出了空界面");
                    continue;
                }

                if (ctrlBefore == ctrl)
                {
                    break;
                }

                ctrlBefore.CloseRoot();
            }

            ctrl.CloseRoot();
        }

        /// <summary>
        /// 关闭页面，只关闭它自己，不影响其他界面。（也可以关闭栈外打开的界面，但如果确定是栈外打开的，直接使用CloseWindowWithoutStack更高效）
        /// </summary>
        public void CloseWindowOnly(string windowName)
        {
            var ctrl = GetCtrl<UICtrlBase>(windowName);
            var layer = ConfigManager.Tables.CfgUI[windowName].Layer;
            _cachePopViewCtrl.Clear();
            while (_layerStacks[layer].Count != 0)
            {
                var ctrlBefore = _layerStacks[layer].Pop();
                if (!ctrlBefore)
                {
                    Debug.LogWarning("弹出了空界面");
                    continue;
                }

                if (ctrlBefore == ctrl)
                {
                    break;
                }

                _cachePopViewCtrl.Add(ctrlBefore);
            }

            ctrl.CloseRoot();
            //其余弹出的界面重新添加回栈
            for (var i = _cachePopViewCtrl.Count - 1; i >= 0; i--)
            {
                _layerStacks[layer].Push(_cachePopViewCtrl[i]);
            }
        }

        /// <summary>
        /// 关闭栈外打开的页面
        /// </summary>
        public void CloseWindowWithoutStack(string windowName)
        {
            var ctrl = GetCtrl<UICtrlBase>(windowName);
            ctrl.CloseRoot();
        }

        /// <summary>
        /// 关闭指定层级的所有页面
        /// </summary>
        public void CloseLayerWindows(string layerName)
        {
            var windowsStack = _layerStacks[layerName];
            while (windowsStack.Count != 0)
            {
                windowsStack.Pop().CloseRoot();
            }
        }

        /// <summary>
        /// 销毁窗口
        /// </summary>
        public void DestroyWindow(string windowName)
        {
            if (!_allViews.Remove(windowName, out var ctrl))
            {
                return;
            }

            var layer = ConfigManager.Tables.CfgUI[windowName].Layer;
            while (_layerStacks[layer].Count != 0)
            {
                var ctrlBefore = _layerStacks[layer].Pop();
                if (ctrlBefore == ctrl)
                {
                    break;
                }
                
                ctrlBefore.CloseRoot();
                ctrlBefore.OnClear();
            }

            Object.Destroy(ctrl.gameObject);
        }

        /// <summary>
        /// 关闭所有页面
        /// </summary>
        public void CloseAllWindows()
        {
            foreach (var (_, value) in _layerStacks)
            {
                if (value.Count == 0)
                {
                    continue;
                }

                while (value.Count > 0)
                {
                    var ctrl = value.Pop();
                    ctrl.CloseRoot();
                }
            }
        }

        /// <summary>
        /// 获取ui的controller
        /// </summary>
        public T GetCtrl<T>(string windowName, params object[] param) where T : UICtrlBase
        {
            if (_allViews.TryGetValue(windowName, out var view))
            {
                return (T) view;
            }

            return CreatNewView<T>(windowName, param);
        }

        /// <summary>
        /// 获取ui的controller，没有则返回null
        /// </summary>
        public T GetCtrlWithoutCreate<T>(string windowName) where T : UICtrlBase
        {
            if (_allViews.TryGetValue(windowName, out var view))
            {
                return (T) view;
            }

            return null;
        }

        /// <summary>
        /// 检测页面是否在打开状态
        /// </summary>
        public bool CheckViewActiveInHierarchy(string windowName)
        {
            if (!_allViews.TryGetValue(windowName, out var view))
            {
                return false;
            }

            return view && view.gameObject.activeInHierarchy;
        }
        
        /// <summary>
        /// 初始化HUD
        /// </summary>
        private void InitHUD()
        {
            var hudCfg = ConfigManager.Tables.CfgUI["HUD"];
            var hudRoot = Object.Instantiate(AssetManager.Instance.LoadAssetGameObject(hudCfg.UiPath), _uiRoot);
            var canvas = hudRoot.GetComponent<Canvas>();
            canvas.sortingOrder = hudCfg.SortOrder;
            HUDManager.Instance.Init(hudRoot.transform);
        }

        /// <summary>
        /// 创建一个新的ui
        /// </summary>
        private T CreatNewView<T>(string windowName, params object[] param) where T : UICtrlBase
        {
            var rowCfgUi = _cfgUI[windowName];
            var rootObj = Object.Instantiate(AssetManager.Instance.LoadAsset<GameObject>(rowCfgUi.UiPath), _layers[rowCfgUi.Layer]);
            rootObj.SetActive(false);
            var canvas = rootObj.GetComponent<Canvas>();
            canvas.sortingOrder = rowCfgUi.SortOrder;
            var ctrlNew = rootObj.GetComponent<T>();;
            if (!ctrlNew)
            {
                Debug.LogError("找不到viewObj挂载的ctrl" + rootObj.name);
                return null;
            }

            _allViews.Add(windowName, ctrlNew);
            ctrlNew.OnInit(param);
            ctrlNew.BindEvent();
            return ctrlNew;
        }
    }
}