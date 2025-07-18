// ******************************************************************
//@file         EventManager.cs
//@brief        事件系统
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:25:12
// ******************************************************************

namespace Yu
{
    public class EventManager : BaseSingleTon<EventManager>
    {
        public delegate void YuEvent();
        public delegate void YuEvent<in T1>(T1 t1);
        public delegate void YuEvent<in T1, in T2>(T1 t1, T2 t2);
        public delegate void YuEvent<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);
        
        private readonly EventManagerCompBase _comp; //具体逻辑实现类


        /// <summary>
        /// 构造，此处修改具体逻辑实现类
        /// DefaultEventManagerComp 无优先度
        /// PriorityEventManagerComp 带优先度(有订阅性能开销)
        /// </summary>
        public EventManager()
        {
            _comp = new PriorityEventManagerComp();
        }

        public string LogStr(EventName eventName, int paramCount)
        {
            return ((PriorityEventManagerComp)_comp).LogStr(eventName, paramCount);
        }
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        public void AddListener(EventName eventName, YuEvent listener, int priority = 0)=>_comp.AddListener(eventName, listener, priority);
        public void AddListener<T1>(EventName eventName, YuEvent<T1> listener, int priority = 0) => _comp.AddListener(eventName, listener, priority);
        
        public void AddListener<T1, T2>(EventName eventName, YuEvent<T1, T2> listener, int priority = 0) => _comp.AddListener(eventName, listener, priority);
        public void AddListener<T1, T2, T3>(EventName eventName, YuEvent<T1, T2, T3> listener, int priority = 0) => _comp.AddListener(eventName, listener, priority);

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void RemoveListener(EventName eventName, YuEvent listener) => _comp.RemoveListener(eventName, listener);

        public void RemoveListener<T1>(EventName eventName, YuEvent<T1> listener) => _comp.RemoveListener(eventName, listener);

        public void RemoveListener<T1, T2>(EventName eventName, YuEvent<T1, T2> listener) => _comp.RemoveListener(eventName, listener);
        
        public void RemoveListener<T1, T2, T3>(EventName eventName, YuEvent<T1, T2, T3> listener) => _comp.RemoveListener(eventName, listener);

        /// <summary>
        /// 派发事件
        /// </summary>
        public void Dispatch(EventName eventName) => _comp.Dispatch(eventName);

        public void Dispatch<T1>(EventName eventName, T1 param) => _comp.Dispatch(eventName, param);

        public void Dispatch<T1, T2>(EventName eventName, T1 param1, T2 param2) => _comp.Dispatch(eventName, param1, param2);
        
        public void Dispatch<T1, T2, T3>(EventName eventName, T1 param1, T2 param2, T3 param3) => _comp.Dispatch(eventName, param1, param2, param3);
    }
}