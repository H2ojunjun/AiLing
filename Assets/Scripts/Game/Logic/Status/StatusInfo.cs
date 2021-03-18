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
        [LabelText("状态列表")]
        [ListDrawerSettings(Expanded = true,DraggableItems = false,CustomAddFunction ="AddStatus")]
        public List<Status> statusInfoes = new List<Status>();

        [HideInInspector]
        //所有状态枚举的中文名
        public static ValueDropdownList<string> statusCNNames = new ValueDropdownList<string>();

        [HideInInspector]
        public static Dictionary<string, Type> statusTypeDic = new Dictionary<string, Type>();

        private void AddStatus()
        {
            Status sta = new Status();
            statusInfoes.Add(sta);
        }

        [Button("刷新")]
        [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
        public void Refresh()
        {
            GetAllStatusType();
            foreach(var item in statusInfoes)
            {
                item.Refresh();
            }
        }

        public void GetAllStatusType()
        {
            statusCNNames.Clear();
            statusTypeDic.Clear();
            Type t = typeof(EStatus);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attris = Attribute.GetCustomAttribute(field, typeof(GameEnumAttribute), false) as GameEnumAttribute;
                string name = attris.CNName;
                Type type = attris.enumType;
                var enumValue = field.GetValue(null).ToString();
                statusTypeDic.Add(enumValue, type);
                statusCNNames.Add(name, enumValue);
            }
        }
    }

    [Serializable]
    public class Status
    {
        [LabelText("状态类别")]
        [ValueDropdown("statusCNNames")]
        [OnValueChanged("OnChangeStatus")]
        public string status;

        //特定状态枚举内元素的中文名
        public ValueDropdownList<int> specificStatus = new ValueDropdownList<int>();

        [HideInInspector]
        public ValueDropdownList<string> statusCNNames = StatusInfo.statusCNNames;

        [LabelText("状态值")]
        [ValueDropdown("specificStatus")]
        public int value;


        public void OnChangeStatus()
        {
            if (status == null)
                return;
            specificStatus.Clear();
            Type t = StatusInfo.statusTypeDic[status];
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

        public void Refresh()
        {
            OnChangeStatus();
        }
    }
}
