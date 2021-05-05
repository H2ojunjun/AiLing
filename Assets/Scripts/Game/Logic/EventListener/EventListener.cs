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
    public class EventListener : MonoBehaviour
    {
        [LabelText("激活")]
        [ReadOnly]
        public bool active = true;

        [LabelText("条件事件列表")]
        [ListDrawerSettings(Expanded = true, CustomAddFunction = "AddConditionEvent", DraggableItems = false)]
        public List<ConditionEvents> conditionEvents = new List<ConditionEvents>();

        //非人为设置(人为无法预知)的事件参数，由程序判断，如发生碰撞后其他物体的碰撞器等
        [HideInInspector]
        public List<GameObject> unartificialPara;

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
            if (!active)
                return;
            foreach (var item in conditionEvents)
            {
                if (item.GetConditionRealResult())
                {
                    foreach (var mod in item.events)
                    {
                        mod.CallEvent();
                    }
                }
            }
        }

        protected virtual void Awake()
        {
            unartificialPara = new List<GameObject>();
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
            if (si == null)
            {
                Debug.LogError("未设置状态！");
                return;
            }
            cod.statusCNNames = si.myStatusCNNames;
            cod.owner = this;
            conditions.Add(cod);
        }

        public bool GetConditionRealResult()
        {
            bool realResult = true;
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
            if (si == null)
                return;
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
        [HideInInspector]
        public ConditionEvents conditionEvents;

        [LabelText("事件绑定物体")]
        [OnValueChanged("OnGameObjectChange")]
        public GameObject eventHandler;

        [LabelText("事件选择器")]
        [ListDrawerSettings(Expanded = true, CustomAddFunction = "AddEventChoose",DraggableItems = false)]
        public List<EventChoose> eventChooses = new List<EventChoose>();

        public void OnGameObjectChange()
        {
            if (eventHandler == null)
            {
                Debug.LogError(conditionEvents.listener.gameObject.name+"上的事件绑定物体为空!");
                return;
            }
            GameEvent[] events = eventHandler.GetComponents<GameEvent>();
            eventNames.Clear();
            foreach (var item in events)
            {
                GameEventInfoAttribute eventInfo = ReflectionHelper.GetGameEventAttribute(item);
                eventNames.Add(eventInfo.eventName, item);
            }
        }

        private bool hasSetStatusChangeCallBack = false;

        private ValueDropdownList<GameEvent> eventNames = new ValueDropdownList<GameEvent>();

        private void AddEventChoose()
        {
            EventChoose ec = new EventChoose();
            ec.mod = this;
            ec.eventNames = eventNames;
            eventChooses.Add(ec);
        }

        public void CallEvent()
        {
            foreach (var eve in eventChooses)
            {
                    GameEvent realEvent = eve.realEvent;
                if (!hasSetStatusChangeCallBack)
                {
                    if (eve.timing == ESetStatusTiming.Start)
                    {
                        realEvent.startCallBack += conditionEvents.SetStatus;
                        hasSetStatusChangeCallBack = true;
                    }
                    else if (eve.timing == ESetStatusTiming.Finish)
                    {
                        realEvent.finishCallBack += conditionEvents.SetStatus;
                        hasSetStatusChangeCallBack = true;
                    }
                }
                if (realEvent.leftTimes == 0)
                    return;
                if (conditionEvents.listener.unartificialPara.Count == 0)
                    realEvent.Excute(null);
                else
                    realEvent.Excute(conditionEvents.listener.unartificialPara);
                realEvent.leftTimes--;
            }
        }

        /// <summary>
        /// 用于编辑器模式下当事件参数信息更改时调用，刷新inspector参数列表
        /// </summary>
        public void Refresh() 
        {
            OnGameObjectChange();
            foreach(var ec in eventChooses)
            {
                ec.eventNames = eventNames;
            }
        }
    }

    [Serializable]
    public class EventChoose
    {
        [HideInInspector]
        public EventModifier mod;

        [LabelText("事件名")]
        [ValueDropdown("eventNames")]
        public GameEvent realEvent;

        public ValueDropdownList<GameEvent> eventNames = new ValueDropdownList<GameEvent>();

        [LabelText("设置后置状态时机")]
        [OnValueChanged("OnTimingChange")]
        public ESetStatusTiming timing = ESetStatusTiming.None;

        private void OnTimingChange()
        {
            int changeNum = 0;
            foreach (var item in mod.eventChooses)
            {
                if (item.timing != ESetStatusTiming.None)
                    changeNum++;
            }
            if (changeNum > 1)
                Debug.LogError("该条件事件系统中有多个事件会设置后置状态，但最多只能有一个，请检查并修改");
        }
    }
}

