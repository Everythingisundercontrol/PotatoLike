// ******************************************************************
//@file         PriorityDispatcher.cs
//@brief        带优先度功能的事件系统逻辑实现组件
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.10 14:42:19
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class PriorityEventManagerComp: EventManagerCompBase
    {
        private readonly Dictionary<EventName, List<(Delegate, int)>> _listenerDict0 = new(); //监听者列表: List<监听者, 优先度>
        private readonly Dictionary<EventName, List<(Delegate, int)>> _listenerDict1 = new();
        private readonly Dictionary<EventName, List<(Delegate, int)>> _listenerDict2 = new();
        private readonly Dictionary<EventName, List<(Delegate, int)>> _listenerDict3 = new();


        public string LogStr(EventName eventName, int paramCount)
        {
            switch (paramCount)
            {
                case 0:
                    _listenerDict0.TryGetValue(eventName, out var list0);
                    return LogForList(list0);
                case 1:
                    _listenerDict0.TryGetValue(eventName, out var list1);
                    return LogForList(list1);
                case 2:
                    _listenerDict0.TryGetValue(eventName, out var list2);
                    return LogForList(list2);
                case 3:
                    _listenerDict0.TryGetValue(eventName, out var list3);
                    return LogForList(list3);
            }

            return "Null";
        }

        private static string LogForList(IEnumerable<(Delegate, int)> list)
        {
            if (list == null)
            {
                return "Null";
            }

            var log = "";
            foreach (var (action, _) in list)
            {
                log += $"{action.Method.Name}, ";
            }

            return log;
        }
        
        public override void AddListener(EventName eventName, EventManager.YuEvent listener, int priority = 0)
        {
            AddListener(eventName, listener, priority, _listenerDict0, DispatchingDict0, listener.Invoke);
        }

        public override void AddListener<T1>(EventName eventName, EventManager.YuEvent<T1> listener, int priority = 0)
        {
            var paramArray = ParamOnDispatch[1];
            AddListener(eventName, listener, priority, _listenerDict1, DispatchingDict1
                , () => listener.Invoke((T1)paramArray[0])); //派发中订阅时，使用闭包
        }

        public override void AddListener<T1, T2>(EventName eventName, EventManager.YuEvent<T1, T2> listener, int priority = 0)
        {
            var paramArray = ParamOnDispatch[2];
            AddListener(eventName, listener, priority, _listenerDict2, DispatchingDict2
                , () => listener.Invoke((T1)paramArray[0], (T2)paramArray[1]));
        }

        public override void AddListener<T1, T2, T3>(EventName eventName, EventManager.YuEvent<T1, T2, T3> listener, int priority = 0)
        {
            var paramArray = ParamOnDispatch[3];
            AddListener(eventName, listener, priority,_listenerDict3, DispatchingDict3
                , () => listener.Invoke((T1)paramArray[0], (T2)paramArray[1], (T3)paramArray[2]));
        }

        public override void RemoveListener(EventName eventName, EventManager.YuEvent listener)
        {
            RemoveListener(eventName, listener, _listenerDict0, DispatchingDict0);
        }

        public override void RemoveListener<T1>(EventName eventName, EventManager.YuEvent<T1> listener)
        {
            RemoveListener(eventName, listener, _listenerDict1, DispatchingDict1);
        }

        public override void RemoveListener<T1, T2>(EventName eventName, EventManager.YuEvent<T1, T2> listener)
        {
            RemoveListener(eventName, listener, _listenerDict2, DispatchingDict2);
        }

        public override void RemoveListener<T1, T2, T3>(EventName eventName, EventManager.YuEvent<T1, T2, T3> listener)
        {
            RemoveListener(eventName, listener, _listenerDict3, DispatchingDict3);
        }

        public override void Dispatch(EventName eventName)
        {
            Dispatch(eventName, _listenerDict0, DispatchingDict0, listener 
                => ((EventManager.YuEvent)listener).Invoke);
        }

        public override void Dispatch<T1>(EventName eventName, T1 param)
        {
            Dispatch(eventName, _listenerDict1, DispatchingDict1, listener => () =>
            {
                ((EventManager.YuEvent<T1>)listener).Invoke(param);
            });
        }

        public override void Dispatch<T1, T2>(EventName eventName, T1 param1, T2 param2)
        {
            Dispatch(eventName, _listenerDict2, DispatchingDict2, listener => () =>
            {
                ((EventManager.YuEvent<T1, T2>)listener).Invoke(param1, param2);
            });
        }

        public override void Dispatch<T1, T2, T3>(EventName eventName, T1 param1, T2 param2, T3 param3)
        {
            Dispatch(eventName, _listenerDict3, DispatchingDict3, listener => () =>
            {
                ((EventManager.YuEvent<T1, T2, T3>)listener).Invoke(param1, param2, param3);
            });
        }
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        private static void AddListener(EventName eventName, Delegate listener, int priority, Dictionary<EventName, List<(Delegate, int)>> listenerDict
            , Dictionary<EventName, int> dispatchingDict, Action invokeOnDispatch)
        {
            if (!listenerDict.TryGetValue(eventName, out var listenerList))
            {
                listenerList = new List<(Delegate, int)>();
                listenerDict.Add(eventName, listenerList);
            }
            
            if (CheckExitListener(listenerList, listener)) //重复订阅
            {
                return;
            }

            var insertIndex = FindInsertIndex(listenerList, priority);
            listenerList.Insert(insertIndex, (listener, priority));
            //派发中
            if (CheckAddListenerOnDispatching(insertIndex, eventName, dispatchingDict))
            {
                invokeOnDispatch.Invoke();
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        private static void RemoveListener(EventName eventName, Delegate listener, Dictionary<EventName, List<(Delegate, int)>> listenerDict
            , Dictionary<EventName, int> dispatchingDict)
        {
            for (var i = 0; i < listenerDict[eventName].Count; i++)
            {
                var (listenerExit, _) = listenerDict[eventName][i];
                if (listenerExit != listener)
                {
                    continue;
                }

                listenerDict[eventName].RemoveAt(i);
                CheckRemoveListenerOnDispatching(i, eventName, dispatchingDict);
                return;
            }

            Debug.LogError($"取消订阅失败，{listener}, 未订阅无参事件: {eventName}");
        }
        
        /// <summary>
        /// 派发事件
        /// </summary>
        private static void Dispatch(EventName eventName, Dictionary<EventName, List<(Delegate, int)>> listenerDict
            , Dictionary<EventName, int> dispatchingDict, Func<Delegate, Action> invokeOnDispatch)
        {
            if (!listenerDict.TryGetValue(eventName, out var listenerList))//无订阅事件
            {
                return;
            }
            
            if (!dispatchingDict.TryAdd(eventName, 0)) //派发index初始为0
            {
                Debug.LogError($"无参事件: {eventName}，发生同名递归派发成环异常。");
                return;
            }
            
            while (dispatchingDict[eventName] < listenerList.Count) //两个数字都会动态变更
            {
                var dispatchingIndex = dispatchingDict[eventName];
                //派发中移除最后一个订阅事件时，预期不越界
                var (listener, _) = listenerList[dispatchingIndex];
                // Debug.Log(listener.Method.Name + listener.Method.GetParameters().Length);
                invokeOnDispatch(listener).Invoke();
                dispatchingDict[eventName]++;
            }

            dispatchingDict.Remove(eventName);
        }
        
        /// <summary>
        /// 注册监听者前检查重复
        /// </summary>
        private static bool CheckExitListener<T>(List<(T, int)> listenerList, T listenerAdd) where T : Delegate
        {
            foreach (var (listener, _) in listenerList)
            {
                if (listener != listenerAdd)
                {
                    continue;
                }
        
                Debug.LogError($"重复添加事件: (类型: {listenerAdd}, 订阅名: {listenerAdd.Method.Name})");
                return true;
            }
        
            return false;
        }
        
        /// <summary>
        /// 派发中订阅事件
        /// </summary>
        private static bool CheckAddListenerOnDispatching(int insertIndex, EventName eventName, Dictionary<EventName, int> dispatchingDict)
        {
            if (!dispatchingDict.TryGetValue(eventName, out var dispatchingIndex))
            {
                return false;
            }

            //派发中
            if (insertIndex > dispatchingIndex) //插入位置在i后面，for循环会派发到的
            {
                return false;
            }

            //如果新插入的位置比i小，即优先度更高，要先派发，插入位置后面元素后移一位，即i++
            dispatchingDict[eventName]++;
            return true;
        }

        /// <summary>
        /// 派发中取消订阅事件
        /// </summary>
        private static void CheckRemoveListenerOnDispatching(int removeIndex, EventName eventName, Dictionary<EventName, int> dispatchingDict)
        {
            if (!dispatchingDict.TryGetValue(eventName, out var dispatchingIndex))
            {
                return;
            }

            //派发中
            if (removeIndex > dispatchingIndex) //插入位置在i后面，正常循环，不处理
            {
                return;
            }

            //如果新插入的位置比i小，插入位置后面元素后移一位，即i++
            //取消订阅时先有订阅才能取消，此处dispatchingIndex--预期不会<0
            dispatchingDict[eventName]--;
        }
        
        /// <summary>
        /// 依据优先度获取插入位置，二分查找,dec倒序
        /// </summary>
        private static int FindInsertIndex<T>(List<(T, int)> list, int priority)
        {
            var min = 0;
            var max = list.Count - 1;
            while (min <= max)
            {
                var mid = (min + max) / 2;
                var (_, p) = list[mid];
                if (p < priority)
                {
                    max = mid - 1;
                    continue;
                }

                min = mid + 1;
            }

            return min;
        }
    }
}