using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AiLing
{
    [GameEventInfo("标记点更新")]
    public class MarkEvent : GameEvent
    {
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            EventStart();
            int index=-1;
            for(int i=0;i<GameMarkPointManager.Instance.marks.Count;i++)
            {
                if (GameMarkPointManager.Instance.marks[i].name == gameObject.name)
                    index = i+1;
            }
            if (index == -1)
            {
                Debug.LogError("找不到标记点!请检查该gameobject是否在GameMarkPointManager.Instance.marks中");
                return;
            }
            GameProcesser.Instance.currGameModel.mark = index;
            GameProcesser.Instance.SaveGameAsyn();
            EventEnd();
            Debug.Log("mark!"+gameObject.name);
        }

        public override GameEventInfoAttribute GetEventAttribute()
        {
            Type t = typeof(MarkEvent);
            GameEventInfoAttribute attri = t.GetCustomAttribute<GameEventInfoAttribute>();
            return attri;
        }
    }
}
