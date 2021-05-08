using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


namespace AiLing
{
    [GameEventInfo("改变传送门方向")]
    public class ChangePortalDirectionEvent : GameEvent
    {
        [LabelText("是否正向")]
        public bool isPositive;
        [LabelText("目标")]
        public Portal target;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if(target == null)
            {
                DebugHelper.LogError(gameObject.name + "上的事件没有传送门");
                return;
            }
            EventStart();
            target.isPositive = isPositive;
            EventEnd();
        }
    }
}
