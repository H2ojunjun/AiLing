using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    //游戏中事件信息的attribute，方便做监听器在inspecter面板中的表现
    [AttributeUsage(AttributeTargets.Class)]
    public class GameEventInfoAttribute : Attribute
    {
        //事件名
        public string eventName;

        //是否包含非人为设置参数
        public bool isUnArtificial;
        public GameEventInfoAttribute(string eventName,bool isUnArtificial = false)
        {
            this.eventName = eventName;
            this.isUnArtificial = isUnArtificial;
        }
    }

    [Serializable]
    public struct GameEventInfo
    {
        //事件类型
        public Type eventType;
        //基本数据类型参数数量，人为设置
        public int parameterNum;
        //基本数据类型参数名，人为设置
        public string[] paraNames;
        //gameobject数据类型参数数量，人为设置
        public int gameobjectNum;
        //gameobject数据类型参数名，人为设置
        public string[] gameObjNames;
        //是否包含非人为设置参数
        public bool isUnArtificial;

        public GameEventInfo(Type evenType, int parameterNum=0, string[] paraNames=null, int gameobjectNum=0, 
            string[] gameObjNames=null, bool isUnArtificial = false)
        {
            this.eventType = evenType;
            this.parameterNum = parameterNum;
            this.paraNames = paraNames;
            this.gameobjectNum = gameobjectNum;
            this.gameObjNames = gameObjNames;
            this.isUnArtificial = isUnArtificial;
        }
    }

    public class GameEvent : MonoBehaviour
    {
        //保存当前游戏中所有正在被调用的事件
        public static List<GameEvent> currEvents = new List<GameEvent>();

        //返回值，如果事件的Call函数被调用后有返回值则保存在此处
        [HideInInspector]
        public object[] returns;

        //该事件所剩下的可触发次数,int.MinValue表示该事件还未被设置leftTimes
        [LabelText("可触发次数（-1表示无限）")]
        public int leftTimes = 0;

        private Action _startCallBack;

        private Action _finishCallBack;

        //事件的开始callback
        public event Action startCallBack { add { _startCallBack += value; }remove { _startCallBack -= value; } }

        //事件的结束callback
        public event Action finishCallBack { add { _finishCallBack += value; } remove { _finishCallBack -= value; } }
        public virtual void Excute(params object[] unartPara)
        {
            currEvents.Add(this);
        }
        
        public virtual GameEventInfoAttribute GetEventAttribute()
        {
            return null;
        }

        /// <summary>
        /// 开始事件，特定时间点调用
        /// </summary>
        public virtual void EventStart()
        {
            _startCallBack?.Invoke();
        }

        /// <summary>
        /// 结束事件，由程序在特定的时间点（时间完成时）调用
        /// </summary>
        public virtual void EventEnd()
        {
            _finishCallBack?.Invoke();
            currEvents.Remove(this);
        }
    }
}

