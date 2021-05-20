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
        public bool showAnim = true;
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
                DebugHelper.LogError("找不到标记点!请检查该gameobject是否在GameMarkPointManager.Instance.marks中");
                return;
            }
            GameProcesser.Instance.ChangeCurrMark(index);
            GameProcesser.Instance.SaveGameAsyn();
            if(showAnim)
                UIManager.Instance.CreateAndShow<UISaving>();
            EventEnd();
            DebugHelper.Log("mark!"+gameObject.name);
        }
    }
}

