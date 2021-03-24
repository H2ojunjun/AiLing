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
        [ReadOnly]
        public int id;

        [LabelText("状态列表")]
        [ListDrawerSettings(Expanded = true,DraggableItems = false,CustomAddFunction ="AddStatus")]
        public List<Status> statusInfoes = new List<Status>();

        [HideInInspector]
        //该statusInfo所持有的状态枚举的中文名
        public List<string> myStatusCNNames = new List<string>();

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
    }

    [Serializable]
    public class Status
    {
        [HideInInspector]
        public StatusInfo owner;

        [HideLabel]
        [ValueDropdown("statusCNNames")]
        [OnValueChanged("OnChangeStatus")]
        public string status;

        [ReadOnly]
        [LabelText("状态类别")]
        public string statusCN;

        //特定状态枚举内元素的中文名
        public ValueDropdownList<int> specificStatus = new ValueDropdownList<int>();

        [HideInInspector]
        public static ValueDropdownList<string> statusCNNames = new ValueDropdownList<string>();

        [HideInInspector]
        public static Dictionary<string, GameEnumAttribute> statusAttributeDic = new Dictionary<string, GameEnumAttribute>();

        [LabelText("状态值")]
        [ValueDropdown("specificStatus")]
        public int value;

        [LabelText("参照物列表")]
        public List<StatusReference> statusReferences = new List<StatusReference>();

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

        public void OnChangeStatus()
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
#if UNITY_EDITOR
            OnChangeStatus();
#endif
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
    }
}
