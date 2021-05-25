using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Reflection;

namespace AiLing
{
    public class StatusInfo : MonoBehaviour
    {
        public static List<StatusInfo> allStatusInfo = new List<StatusInfo>();

        [LabelText("状态列表")]
        [ListDrawerSettings(Expanded = true,DraggableItems = false,CustomAddFunction ="AddStatus")]
        public List<Status> statusInfoes = new List<Status>();

        [HideInInspector]
        //该statusInfo所持有的状态枚举的中文名
        public List<string> myStatusCNNames = new List<string>();

        private void Start()
        {
            allStatusInfo.Add(this);
            LoadStatus();
            Init();
        }

        private void Init()
        {
            foreach(var sta in statusInfoes)
            {
                sta.Init();
            }
        }

        private void AddStatus()
        {
            Status sta = new Status();
            sta.owner = this;
            statusInfoes.Add(sta);
        }

        [Button("刷新")]
        [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
        public void Refresh()
        {
#if UNITY_EDITOR
            Status.InitAllStatus();
            myStatusCNNames.Clear();
            foreach (var item in statusInfoes)
            {
                item.Refresh();
                if (string.IsNullOrEmpty(item.status))
                    continue;
                var attri = Status.statusAttributeDic[item.status];
                myStatusCNNames.Add(attri.CNName);
            }
#endif
        }

        //处理状态信息，缓存默认值
        public void DealStatusInfo()
        {
            LogicContainer container = gameObject.GetComponent<LogicContainer>();
            if (container == null)
                container = gameObject.AddComponent<LogicContainer>();
            foreach (var item in statusInfoes)
            {
                item.DealStatus();
            }
        }

        public void StartReference()
        {
            foreach (var item in statusInfoes)
            {
                item.StartReference();
            }
        }

        public void RevertReference()
        {
            foreach (var item in statusInfoes)
            {
                item.RevertValue();
                item.StartReference();
            }

        }

        public void SaveStatus()
        {
            Dictionary<string, int> statusDic = new Dictionary<string, int>();
            foreach (var item in statusInfoes)
            {
                statusDic.Add(item.statusCN,item.value);
            }
            GameProcesser.Instance.ChangeItemInStatusDic(gameObject.name, statusDic);
        }

        public void LoadStatus()
        {
            Dictionary<string, int> statusDic = GameProcesser.Instance.GetItemInStatusDic(gameObject.name);
            if (statusDic != null)
            {
                foreach (var sta in statusInfoes)
                {
                    string statusCN = sta.statusCN;
                    if (statusDic.ContainsKey(statusCN))
                    {
                        sta.ChangeValuePurely(statusDic[statusCN]);
                    }
                    else
                        statusDic.Remove(statusCN);
                }
            }
            DealStatusInfo();
            StartReference();
        }
    }

    [Serializable]
    public class Status
    {
        [HideInInspector]
        public StatusInfo owner;

        [LabelText("是否回归默认值")]
        public bool isBack;

        [HideLabel]
        [ValueDropdown("statusCNNames")]
        [OnValueChanged("OnChangeStatus")]
        public string status;

        [ReadOnly]
        [LabelText("状态类别")]
        public string statusCN;

        [SerializeField]
        [LabelText("状态值")]
        [ValueDropdown("specificStatus")]
        private int _value;

        private int _originValue;

        //特定状态枚举内元素的中文名
        public ValueDropdownList<int> specificStatus = new ValueDropdownList<int>();

        [HideInInspector]
        public static ValueDropdownList<string> statusCNNames = new ValueDropdownList<string>();

        [HideInInspector]
        public static Dictionary<string, GameEnumAttribute> statusAttributeDic = new Dictionary<string, GameEnumAttribute>();

        [LabelText("参照物列表")]
        [ListDrawerSettings(DraggableItems = false)]
        public List<StatusReference> statusReferences = new List<StatusReference>();

        public int value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnStatusValueChange();
            }
        }

        public static void InitAllStatus()
        {
            statusCNNames.Clear();
            statusAttributeDic.Clear();
            Type t = typeof(EStatus);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                GameEnumAttribute attris = Attribute.GetCustomAttribute(field, typeof(GameEnumAttribute), false) as GameEnumAttribute;
                string name = attris.CNName;
                Type type = attris.enumType;
                var enumValue = field.GetValue(null).ToString();
                statusAttributeDic.Add(enumValue, attris);
                statusCNNames.Add(name, enumValue);
            }
        }

        public void OnChangeStatus(string newSta)
        {
            if (status == null)
                return;
            specificStatus.Clear();
            statusReferences.Clear();
            var attri = statusAttributeDic[status];
            statusCN = attri.CNName;
            Type t = attri.enumType;
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attris = Attribute.GetCustomAttribute(field, typeof(GameEnumAttribute), false) as GameEnumAttribute;
                string name = attris.CNName;
                var enumValue = field.GetValue(null);
                int intValue = (int)enumValue;
                specificStatus.Add(name, intValue);
                StatusReference sr = new StatusReference();
                sr.CNname = name;
                statusReferences.Add(sr);
            }
        }

        public void Refresh()
        {
            if (status == null)
                return;
            specificStatus.Clear();
            var attri = statusAttributeDic[status];
            statusCN = attri.CNName;
            Type t = attri.enumType;
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attris = Attribute.GetCustomAttribute(field, typeof(GameEnumAttribute), false) as GameEnumAttribute;
                string name = attris.CNName;
                var enumValue = field.GetValue(null);
                int intValue = (int)enumValue;
                specificStatus.Add(name, intValue);
            }
        }

        public void OnStatusValueChange()
        {
            if (isBack)
                return;
            DealStatus();
            owner.SaveStatus();
        }

        public void ChangeValuePurely(int value)
        {
            this._value = value;
        }

        public void DealStatus()
        {
            StatusReference sr = statusReferences[value - 1];
            GameObject referenceObj = sr.reference;
            if (referenceObj == null)
            {
                //DebugHelper.LogError(owner.name + " 状态：" + statusCN + " 值：" + value + "没有引用物体");
                return;
            }
            LogicContainer container = owner.GetComponent<LogicContainer>();
            foreach (var content in sr.referenceContent)
            {
                GameEnumAttribute attri = ReflectionHelper.GetGameCacheEnumAttribute(content);
                if (attri == null)
                    continue;
                GameReferenceCache referenceCache = container.GetSingleComponent(attri.enumType,statusCN) as GameReferenceCache;
                if(referenceCache == null)
                    referenceCache = container.AddNormalComponent(attri.enumType,statusCN) as GameReferenceCache;
                referenceCache.Init(referenceObj);
                referenceCache.Cache();
            }
        }

        //开始引用
        public void StartReference()
        {
            StatusReference sr = statusReferences[value - 1];
            GameObject referenceObj = sr.reference;
            if (referenceObj == null)
            {
                //DebugHelper.LogError(owner.name + " 状态：" + statusCN + " 值：" + value + "没有引用物体");
                return;
            }
            LogicContainer container = owner.GetComponent<LogicContainer>();
            foreach (var content in sr.referenceContent)
            {
                GameEnumAttribute attri = ReflectionHelper.GetGameCacheEnumAttribute(content);
                if (attri == null)
                    continue;
                GameReferenceCache referenceCache = container.GetSingleComponent(attri.enumType, statusCN) as GameReferenceCache;
                if (referenceCache == null)
                    return;
                referenceCache.Read();

            }
        }

        public void Init()
        {
            _originValue = _value;
        }

        public void RevertValue()
        {
            _value = _originValue;
        }
    }

    [Serializable]
    public class StatusReference
    {
        [ReadOnly]
        [LabelText("状态名")]
        public string CNname;

        [LabelText("参照物")]
        public GameObject reference;

        [LabelText("参照内容")]
        public List<EReferenceContent>  referenceContent;
    }
}
