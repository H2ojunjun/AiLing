using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    //用于处理游戏各种逻辑的纯C#对象
    public abstract class LogicComponent 
    {
        //已经分配的id的最大值
        private static int _allocID = -1;

        //空余的id位置，方便组件被移除后其他组件使用id
        private static Queue<int> _idFreeSeats= new Queue<int>();

        private static List<LogicComponent> _allComponents = new List<LogicComponent>();

        public int id;

        public LogicContainer container;

        //标记，当一个容器中可能出现多个同种组件的时候需要用该字段进行区分
        public string mark;

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

        //得到逻辑组件需要的初始化信息
        public abstract void Init(GameObject obj);

        //RemoveLogicComponent函数后调用
        public virtual void OnDestory()
        {
            _idFreeSeats.Enqueue(id);
            if (_allComponents == null || _allComponents.Count == 0)
                return;
            _allComponents[id] = null;
        }
    }
}
