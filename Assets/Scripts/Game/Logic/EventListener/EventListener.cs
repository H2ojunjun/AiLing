using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Reflection;
using UnityEditor;
using Sirenix.Serialization;

namespace AiLing
{
    //[ExecuteInEditMode]
    public class EventListener : SerializedMonoBehaviour
    {
        [OdinSerialize]
        //存储事件的中文名对应的Tuple结构，其中放着事件的参数信息（与GameEventInfoAttribute中的参数信息一致）和事件的类型Type，在运行时可以不必再用string去反射得到Type，优化程序
        //public static Dictionary<string, Tuple<Type, int, string[], int, string[], bool>> EventTypeDic = new Dictionary<string, Tuple<Type, int, string[], int, string[], bool>>();
        public static Dictionary<string, GameEventInfo> EventTypeDic = new Dictionary<string, GameEventInfo>();

        //[TableList(AlwaysExpanded = true)]
        [LabelText("事件列表")]
        [ListDrawerSettings(Expanded = true,CustomAddFunction ="AddEventModifier")]
        public List<EventModifier> events = new List<EventModifier>();

        //非人为设置(人为无法预知)的事件参数，由程序判断，如发生碰撞后其他物体的碰撞器等
        [HideInInspector]
        public List<object> unartificialPara;

        /// <summary>
        /// 初始化EventTypeDic
        /// </summary>
        [PropertyOrder(0)]
        [PropertySpace]
        [GUIColor(0.3f, 1f, 0.5f, 1)]
        [Button("初始化事件系统", ButtonSizes.Medium)]
        private void InitEventList()
        {
            EventModifier.eventNames.Clear();
            EventTypeDic.Clear();
            var types = ReflectionHelper.GetSubtypes(typeof(GameEvent).Assembly, typeof(GameEvent), false);
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<GameEventInfoAttribute>();
                if (attribute != null)
                {
                    string eventName = attribute.eventName;
                    int parameterNum = attribute.parameterNum;
                    string[] paraNames = attribute.paraNames;
                    int gameobjectNum = attribute.gameobjectNum;
                    string[] gameObjNames = attribute.gameObjNames;
                    bool isUnArtificial = attribute.isUnArtificial;
                    EventModifier.eventNames.Add(eventName);
                    GameEventInfo info = new GameEventInfo(type, parameterNum, paraNames, gameobjectNum, gameObjNames, isUnArtificial);
                    //Tuple<Type, int, string[], int, string[], bool> tup = new Tuple<Type, int, string[], int, string[], bool>(type, parameterNum, paraNames, gameobjectNum, gameObjNames, isUnArtificial);
                    //EventTypeDic.Add(eventName, tup);
                    EventTypeDic.Add(eventName, info);
                }
            }
        }

        //[Button("注入参数")]
        //[GUIColor(0.3f, 1f, 0.5f, 1)]
        //private void InjectParameter()
        //{
        //    foreach(var item in events)
        //    {
        //        item.InjectPara();
        //    }
        //    Debug.Log("注入所有参数成功!");
        //}

        [Button("刷新")]
        [GUIColor(0.3f, 1f, 0.5f, 1)]
        private void RefreshAll()
        {
            foreach (var item in events)
            {
                item.Refresh();
            }
            Debug.Log("刷新成功");
        }

        public virtual void CallEvent()
        {
            foreach(var item in events)
            {
                item.CallEvent();
            }
        }

        private void Awake()
        {
            InitEventList();
            unartificialPara = new List<object>();
            Debug.Log(EventTypeDic.Values);
            RefreshAll();
        }

        private void AddEventModifier()
        {
            EventModifier mod = new EventModifier();
            mod.listener = this;
            events.Add(mod);
        }
    }

    [Serializable]
    public class EventModifier
    {
        //用于下拉列表的list
        [HideInInspector]
        public static List<string> eventNames = new List<string>();

        [OdinSerialize]
        //最后传递给event的参数，包括三部分：
        //1.程序判断参数，即人工无法提前预知的参数
        //2.人工可预知的基本数据类型参数
        //3.人工可预知的非基本数据类型参数
        public List<object> paras = new List<object>();

        [LabelText("事件名")]
        [VerticalGroup("事件名")]
        [ValueDropdown("eventNames")]
        //[TableColumnWidth(50)]
        [OnValueChanged("ChangeEvent", false)]
        public string gameEvent;

        [LabelText("可触发次数(-1表示无限)")]
        public int evntTime = 1;

        //基本数据类型参数和string类型参数
        //[HideLabel]
        [HorizontalGroup("事件参数")]
        [LabelText("普通参数")]
        //[VerticalGroup("普通参数")]
        //[TableColumnWidth(200)]
        [ListDrawerSettings(Expanded = true, HideAddButton = true,HideRemoveButton =true)]
        public List<ParaInfo<string>> normalPara = new List<ParaInfo<string>>();

        //gameobject类型参数
        //[HideLabel]
        [HorizontalGroup("事件参数")]
        [LabelText("游戏物体参数")]
        //[VerticalGroup("游戏物体参数")]
        //[TableColumnWidth(200)]
        [ListDrawerSettings(Expanded = true, HideAddButton = true, HideRemoveButton = true)]
        public List<ParaInfo<GameObject>> objPara = new List<ParaInfo<GameObject>>();

        [HideInInspector]
        public bool isUnArtificial;

        [HideInInspector]
        public EventListener listener;

        private Type eventType;

        private GameEvent realEvent;

        public void CallEvent()
        {
            if(realEvent == null)
                realEvent = Activator.CreateInstance(eventType) as GameEvent;
            InjectPara();
            if(realEvent.leftTimes == int.MinValue)
                realEvent.leftTimes = evntTime;
            //InjectUnArtificial();
            if (realEvent.leftTimes == 0)
                return;
            if (listener.unartificialPara.Count == 0)
                realEvent.Excute(paras.ToArray(), null);
            else
                realEvent.Excute(paras.ToArray(),listener.unartificialPara);
            realEvent.leftTimes--;
        }

        private void ChangeEvent()
        {
            if (string.IsNullOrEmpty(gameEvent))
            {
                Debug.LogError("请选择事件名！");
                return;
            }
            //normalPara.Clear();
            //Tuple<Type, int, string[], int, string[], bool> eventInfo;
            GameEventInfo eventInfo;
            if (!EventListener.EventTypeDic.TryGetValue(gameEvent, out eventInfo))
            {
                Debug.LogError("找不到该事件，请联系巫师");
                return;
            }
            else
            {
                normalPara.Clear();
                for (int i = 0; i < eventInfo.parameterNum; i++)
                {
                    ParaInfo<string> info = new ParaInfo<string>(eventInfo.paraNames[i]);
                    normalPara.Add(info);
                }
                objPara.Clear();
                for (int i = 0; i < eventInfo.gameobjectNum; i++)
                {
                    ParaInfo<GameObject> info = new ParaInfo<GameObject>(eventInfo.gameObjNames[i]);
                    objPara.Add(info);
                }
            }
            //if (!EventListener.EventTypeDic.TryGetValue(gameEvent, out eventInfo))
            //{
            //    Debug.LogError("找不到该事件，请联系巫师");
            //    return;
            //}
            //else
            //{
            //    isUnArtificial = eventInfo.Item6;
            //    eventType = eventInfo.Item1;
            //    for (int i = 0; i < eventInfo.Item2; i++)
            //    {
            //        ParaInfo<string> info = new ParaInfo<string>(eventInfo.Item3[i]);
            //        normalPara.Add(info);
            //    }
            //    for (int i = 0; i < eventInfo.Item4; i++)
            //    {
            //        ParaInfo<GameObject> info = new ParaInfo<GameObject>(eventInfo.Item5[i]);
            //        objPara.Add(info);
            //    }
            //}
        }

        /// <summary>
        /// 解析并且注入人工参数到paras中，该方法只会在事件第一次触发调用！以求减轻运行时解析字符串获取参数的消耗
        /// </summary>
        public void InjectPara()
        {
            if (paras.Count > 0)
                return;
            //List<object> paraList = new List<object>();
            paras.Clear();
            foreach (var item in normalPara)
            {
                object o = GetNormalPara(item.paraNameType, item.parameter);
                paras.Add(o);
            }
            foreach(var item in objPara)
            {
                object o = item.parameter;
                paras.Add(o);
            }
            if (paras.Count == 0)
                paras = null;
            //foreach(var para in paras)
            //    Debug.Log(para);
        }

        //public void InjectUnArtificial()
        //{
        //    if (isUnArtificial)
        //        paras.Add(listener.unartificialPara);
        //}

        public object GetNormalPara(string paraNameType,string para)
        {
            string[] nameType = paraNameType.Split(':');
            switch (nameType[1])
            {
                case "int":
                    int iArg = int.Parse(para);
                    object iBox = iArg;
                    return iBox;
                case "float":
                    float fArg = float.Parse(para);
                    object fBox = fArg;
                    return fBox;
                case "bool":
                    int bArg = int.Parse(para);
                    bool b = false;
                    if (bArg == 1)
                        b = true;
                    object bBox = b;
                    return bBox;
                case "string":
                    object sBox = para;
                    return sBox;
                default:
                    return null;
            }
        }

        public void Refresh()
        {
            if (string.IsNullOrEmpty(gameEvent))
                return;
            //Tuple<Type, int, string[], int, string[], bool> tup;
            GameEventInfo eventInfo;
            if (EventListener.EventTypeDic.TryGetValue(gameEvent, out eventInfo))
            {
                eventType = eventInfo.eventType;
                isUnArtificial = eventInfo.isUnArtificial;
                Debug.Log("eventType" + eventType.Name);

                Dictionary<string, string> tempNormalDic = new Dictionary<string, string>();
                foreach(var item in normalPara)
                {
                    tempNormalDic.Add(item.paraNameType, item.parameter);
                }
                normalPara.Clear();
                for (int i = 0; i < eventInfo.parameterNum; i++)
                {
                    ParaInfo<string> info = new ParaInfo<string>(eventInfo.paraNames[i]);
                    string value;
                    if (tempNormalDic.TryGetValue(eventInfo.paraNames[i], out value))
                        info.parameter = value;
                    normalPara.Add(info);
                }

                Dictionary<string, GameObject> tempGameobjDic = new Dictionary<string, GameObject>();
                foreach (var item in objPara)
                {
                    tempGameobjDic.Add(item.paraNameType, item.parameter);
                }
                objPara.Clear();
                for (int i = 0; i < eventInfo.gameobjectNum; i++)
                {
                    ParaInfo<GameObject> info = new ParaInfo<GameObject>(eventInfo.gameObjNames[i]);
                    GameObject value;
                    if (tempGameobjDic.TryGetValue(eventInfo.gameObjNames[i], out value))
                        info.parameter = value;
                    objPara.Add(info);
                }
            }
            else
                Debug.LogError("刷新失败，EventTypeDic中找不到该事件："+gameEvent);
        }
    }

    [Serializable]
    public class ParaInfo<T> where T : class
    {
        //参数名，用于编辑器自定义drawer
        [HideInInspector]
        public string paraNameType;

        [DelayedProperty]
        [CustomValueDrawer("ParameterLable")]
        [OnValueChanged("OnParamterChange")]
        public T parameter;

        public T ParameterLable(T temp, GUIContent label)
        {
            GUIContent pLable = new GUIContent(paraNameType);
            Type t = typeof(T);
            switch (t.Name)
            {
                case "String":
                     return EditorGUILayout.TextField(paraNameType, temp as string) as T;
                case "GameObject":
                    return EditorGUILayout.ObjectField(paraNameType, temp as GameObject, typeof(GameObject), true) as T;
                default:
                    return null;
            }
        }

        public ParaInfo(string info)
        {
            this.paraNameType = info;
        }

        private void OnParamterChange()
        {
            Debug.Log(parameter);
        }
    }
}

