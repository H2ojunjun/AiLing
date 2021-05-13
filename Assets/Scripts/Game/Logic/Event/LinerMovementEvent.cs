using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace AiLing
{
    [GameEventInfo("直线运动")]
    public class LinerMovementEvent : GameEvent
    {
        [LabelText("运动者")]
        public GameObject mover;
        [LabelText("目标点")]
        public GameObject target;
        [LabelText("运动时间")]
        public float time;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if(target == null)
            {
                target = unartPara[0];
                if(target == null)
                {
                    DebugHelper.LogError(gameObject.name+"直线运动事件没有target参数");
                    return;
                }
            }
            if (mover == null)
                mover = gameObject;
            EventStart();
            mover.transform.DOMove(target.transform.position, time, false).onComplete = EventEnd;
        }
    }
}
