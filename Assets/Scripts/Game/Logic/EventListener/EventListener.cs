using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Reflection;
# if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.Serialization;

namespace AiLing
{
    //[ExecuteInEditMode]
    public class EventListener : MonoBehaviour
    {
        //存储事件的中文名对应的结构，其中放着事件的参数信息（与GameEventInfoAttribute中的参数信息一致）和事件的类型Type，在运行时可以去反射
        public static Dictionary<string, GameEventInfo> EventTypeDic = new Dictionary<string, GameEventInfo>();

        [LabelText("条件事件列表")]
        [ListDrawerSettings(Expanded = true, CustomAddFunction = "AddConditionEvent", DraggableItems = false)]
        public List<ConditionEvents> conditionEvents = new List<ConditionEvents>();

        //非人为设置(人为无法预知)的事件参数，由程序判断，如发生碰撞后其他物体的碰撞器等
        [HideInInspector]
        public List<object> unartificialPara;

        [HideInInspector]
        public StatusInfo si;

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
                    EventTypeDic.Add(eventName, info);
                }
            }
        }

        [Button("刷新")]
        [GUIColor(0.3f, 1f, 0.5f, 1)]
        private void RefreshAll()
        {
            foreach (var item in conditionEvents)
            {
                item.listener = this;
                foreach (var eve in item.events)
                {
                    eve.Refresh();
                }
                foreach (var condition in item.conditions)
                {
                    condition.Refresh();
                }
            }
            Debug.Log("刷新成功");
        }

        public virtual void CallEvent()
        {
            foreach (var item in conditionEvents)
            {
                if (item.GetConditionRealResult())
                {
                    foreach (var eve in item.events)
                    {
                        eve.CallEvent();
                    }
                }
            }
        }

        private void Awake()
        {
            InitEventList();
            unartificialPara = new List<object>();
            Debug.Log(EventTypeDic.Values);
            RefreshAll();
        }

        private void AddConditionEvent()
        {
            ConditionEvents condEve = new ConditionEvents();
            condEve.listener = this;
            conditionEvents.Add(condEve);
        }
    }

    [Serializable]
    public class ConditionEvents
    {
        [HideInInspector]
        public EventListener listener;

        [LabelText("状态")]
        public StatusInfo si;

        [LabelText("条件列表")]
        [ListDrawerSettings(Expanded = true, CustomAddFunction = "AddCondition", DraggableItems = false)]
        public List<Condition> conditions = new List<Condition>();

        [LabelText("事件列表")]
        [ListDrawerSettings(Expanded = true, CustomAddFunction = "AddEventModifier", DraggableItems = false)]
        public List<EventModifier> events = new List<EventModifier>();

        private void AddEventModifier()
        {
            EventModifier mod = new EventModifier();
            mod.conditionEvents = this;
            events.Add(mod);
        }

        private void AddCondition()
        {
            Condition cod = new Condition();
            cod.statusCNNames = si.myStatusCNNames;
            cod.owner = this;
            conditions.Add(cod);
        }

        public bool GetConditionRealResult()
        {
            bool realResult = false;
            EGate gate = EGate.None;
            for (int i = 0; i < conditions.Count; i++)
            {
                bool result = conditions[i].GetConditionResult();
                switch (gate)
                {
                    case EGate.None:
                        realResult = result;
                        break;
                    case EGate.And:
                        realResult = realResult && result;
                        break;
                    case EGate.Or:
                        realResult = realResult || result;
                        break;
                    case EGate.XOr:
                        realResult = realResult != result;
                        break;
                    case EGate.WithOr:
                        realResult = realResult == result;
                        break;
                    default:
                        break;
                }
                gate = conditions[i].gate;
            }
            return realResult;
        }

        public void SetStatus()
        {
            Dictionary<string, Status> statusInfoes = new Dictionary<string, Status>();
            foreach (var sta in si.statusInfoes)
            {
                statusInfoes.Add(sta.statusCN, sta);
            }
            foreach (var condi in conditions)
            {
                Status sta;
                if (statusInfoes.TryGetValue(condi.currStatus, out sta))
                {
                    sta.value = condi.nextValue;
                }
                else
                    Debug.LogError("条件中的状态:" + condi.currStatus + "在绑定的StatusInfo中找不到！");
            }
        }
    }

    [Serializable]
    public class Condition
    {
        [HideInInspector]
        public ConditionEvents owner;

        [ValueDropdown("statusCNNames")]
        [OnValueChanged("OnChangeStatus")]
        [HorizontalGroup("condition", Width = 80, LabelWidth = 50)]
        [BoxGroup("condition/当前状态")]
        [HideLabel]
        public string currStatus;

        [ValueDropdown("OperationNames")]
        [HorizontalGroup("condition", Width = 80, LabelWidth = 20)]
        [BoxGroup("condition/操作符")]
        [HideLabel]
        public EOperation operation;

        [ValueDropdown("specificStatus")]
        [HorizontalGroup("condition", Width = 40, LabelWidth = 10)]
        [BoxGroup("condition/值")]
        [HideLabel]
        public int value;

        [ValueDropdown("specificStatus")]
        [HorizontalGroup("condition", Width = 80, LabelWidth = 10)]
        [BoxGroup("condition/触发后的值")]
        [HideLabel]
        public int nextValue;

        [ValueDropdown("gateNames")]
        [HorizontalGroup("condition", Width = 60, LabelWidth = 10)]
        [BoxGroup("condition/连接符")]
        [HideLabel]
        public EGate gate;

        [HideInInspector]
        public List<string> statusCNNames;

        private ValueDropdownList<int> specificStatus = new ValueDropdownList<int>();

        private ValueDropdownList<EGate> gateNames = new ValueDropdownList<EGate>();

        private ValueDropdownList<EOperation> OperationNames = new ValueDropdownList<EOperation>
        {
            { "大于",EOperation.Bigger},
            {"大于等于",EOperation.BiggerEqual},
            {"等于",EOperation.Equal},
            { "小于",EOperation.Lower},
            { "小于等于",EOperation.LowerEqual},
        };

        private void OnChangeStatus()
        {
            statusCNNames = owner.si.myStatusCNNames;
            foreach (var sta in owner.si.statusInfoes)
            {
                if (sta.statusCN == currStatus)
                {
                    specificStatus = sta.specificStatus;
                }
            }
        }

        private void InitAllGate()
        {
            gateNames = new ValueDropdownList<EGate>();
            Type t = typeof(EGate);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attris = Attribute.GetCustomAttribute(field, typeof(GameEnumAttribute), false) as GameEnumAttribute;
                string name = attris.CNName;
                var enumValue = (EGate)field.GetValue(null);
                gateNames.Add(name, enumValue);
            }
        }

        private void InitOperations()
        {
            OperationNames = new ValueDropdownList<EOperation>();
            Type t = typeof(EOperation);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attris = Attribute.GetCustomAttribute(field, typeof(GameEnumAttribute), false) as GameEnumAttribute;
                string name = attris.CNName;
                var enumValue = (EOperation)field.GetValue(null);
                OperationNames.Add(name, enumValue);
            }
        }

        public bool GetConditionResult()
        {
            if (owner.si == null)
            {
                Debug.LogError("没有为该条件列表绑定statusInfo");
                return false;
            }
            foreach (var item in owner.si.statusInfoes)
            {
                if (item.statusCN == currStatus)
                {
                    switch (operation)
                    {
                        case EOperation.Bigger:
                            return item.value > value;
                        case EOperation.BiggerEqual:
                            return item.value >= value;
                        case EOperation.Equal:
                            return item.value == value;
                        case EOperation.Lower:
                            return item.value < value;
                        case EOperation.LowerEqual:
                            return item.value <= value;
                        default:
                            return false;
                    }
                }
            }
            Debug.LogError("绑定的statusInfo中没有与当前状态值相同的状态");
            return false;
        }

        public void Refresh()
        {
#if UNITY_EDITOR
            OnChangeStatus();
            InitOperations();
            InitAllGate();
            owner.si.Refresh();
#endif
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
        [OnValueChanged("OnChangeEvent", false)]
        public string gameEvent;

        [LabelText("可触发次数(-1表示无限)")]
        public int evntTime = 1;

        //基本数据类型参数和string类型参数
        //[HideLabel]
        [HorizontalGroup("事件参数", Width = 150, LabelWidth = 100)]
        [LabelText("普通参数")]
        //[VerticalGroup("普通参数")]
        //[TableColumnWidth(200)]
        [ListDrawerSettings(Expanded = true, HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        public List<ParaInfo<string>> normalPara = new List<ParaInfo<string>>();

        //gameobject类型参数
        //[HideLabel]
        [HorizontalGroup("事件参数", Width = 200, LabelWidth = 50)]
        [LabelText("游戏物体参数")]
        //[VerticalGroup("游戏物体参数")]
        //[TableColumnWidth(200)]
        [ListDrawerSettings(Expanded = true, HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        public List<ParaInfo<GameObject>> objPara = new List<ParaInfo<GameObject>>();

        [LabelText("设置后置状态时机")]
        [OnValueChanged("OnTimingChange")]
        public ESetStatusTiming timing = ESetStatusTiming.None;

        [HideInInspector]
        public bool isUnArtificial;

        [HideInInspector]
        public ConditionEvents conditionEvents;

        private Type eventType;

        private GameEvent realEvent;

        public void CallEvent()
        {
            if (realEvent == null)
            {
                realEvent = Activator.CreateInstance(eventType) as GameEvent;
                if (timing == ESetStatusTiming.Start)
                {
                    realEvent.startCallBack += conditionEvents.SetStatus;
                }
                else if (timing == ESetStatusTiming.Finish)
                {
                    realEvent.finishCallBack += conditionEvents.SetStatus;
                }
            }
            InjectPara();
            if (realEvent.leftTimes == 0)
                return;
            if (realEvent.leftTimes == int.MinValue)
                realEvent.leftTimes = evntTime;
          if (conditionEvents.listener.unartificialPara.Count == 0)
                realEvent.Excute(paras.ToArray(), null);
            else
                realEvent.Excute(paras.ToArray(), conditionEvents.listener.unartificialPara);
            realEvent.leftTimes--;
        }

        private void OnTimingChange()
        {
            int changeNum = 0;
            foreach (var mod in conditionEvents.events)
            {
                if (mod.timing != ESetStatusTiming.None)
                    changeNum++;
            }
            if (changeNum > 1)
                Debug.LogError("该条件事件系统中有多个事件会设置后置状态，但最多只能有一个，请检查并修改");
        }

        private void OnChangeEvent()
        {
            if (string.IsNullOrEmpty(gameEvent))
            {
                Debug.LogError("请选择事件名！");
                return;
            }
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
        }

        /// <summary>
        /// 解析并且注入人工参数到paras中，该方法只会在事件第一次触发调用！以求减轻运行时解析字符串获取参数的消耗
        /// </summary>
        public void InjectPara()
        {
            if (paras.Count > 0)
                return;
            paras.Clear();
            foreach (var item in normalPara)
            {
                object o = GetNormalPara(item.paraNameType, item.parameter);
                paras.Add(o);
            }
            foreach (var item in objPara)
            {
                object o = item.parameter;
                paras.Add(o);
            }
            if (paras.Count == 0)
                paras = null;
        }

        public object GetNormalPara(string paraNameType, string para)
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

        /// <summary>
        /// 用于编辑器模式下当事件参数信息更改时调用，刷新inspector参数列表
        /// </summary>
        public void Refresh()
        {
            if (string.IsNullOrEmpty(gameEvent))
                return;
            GameEventInfo eventInfo;
            if (EventListener.EventTypeDic.TryGetValue(gameEvent, out eventInfo))
            {
                eventType = eventInfo.eventType;
                isUnArtificial = eventInfo.isUnArtificial;
                Debug.Log("eventType" + eventType.Name);
#if UNITY_EDITOR
                Dictionary<string, string> tempNormalDic = new Dictionary<string, string>();
                foreach (var item in normalPara)
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
#endif
            }
            else
                Debug.LogError("刷新失败，EventTypeDic中找不到该事件：" + gameEvent);

        }
    }

    [Serializable]
    public class ParaInfo<T> where T : class
    {
        //参数名，用于编辑器自定义drawer
        [HideInInspector]
        public string paraNameType;

        //[DelayedProperty]
        [CustomValueDrawer("ParameterLable")]
        public T parameter;

#if UNITY_EDITOR
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
#endif

        public ParaInfo(string info)
        {
            this.paraNameType = info;
        }
    }
}

