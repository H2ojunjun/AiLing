using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public abstract class LogicComponent 
    {
        //已经分配的id的最大值
        private static int _allocID = -1;

        //空余的id位置，方便组件被移除后其他组件使用id
        private static Queue<int> _idFreeSeats= new Queue<int>();

        private static List<LogicComponent> _allComponents = new List<LogicComponent>();

        public int id;

        public LogicContainer container;

        public static LogicComponent GetComponentGlobally(int id)
        {
            return _allComponents[id];
        }

        public LogicComponent()
        {

        }

        //AddLogicComponent函数后调用
        public virtual void OnCreate()
        {
            if (_idFreeSeats.Count == 0 || _idFreeSeats == null)
            {
                this.id = ++_allocID;
                _allComponents.Add(this);
            }

            else
            {
                int seat = _idFreeSeats.Dequeue();
                this.id = seat;
                _allComponents[seat] = this;
            }
        }

        //RemoveLogicComponent函数后调用
        public virtual void OnDestory()
        {
            _idFreeSeats.Enqueue(id);
            _allComponents[id] = null;
        }
    }
}
