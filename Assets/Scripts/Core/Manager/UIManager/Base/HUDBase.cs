// ******************************************************************
//@file         HUDBase.cs
//@brief        HUD界面基类
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.22 14:38:51
// ******************************************************************

using System;
using UnityEngine;

namespace Yu
{
    public class HUDBase : MonoBehaviour
    {
        protected bool IsActive => gameObject.activeInHierarchy;
        
        
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit()
        {
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        public virtual void BindEvent()
        {

        }

        /// <summary>
        /// 关闭HUD
        /// </summary>
        public virtual void CloseRoot()
        {
        }
    }
}