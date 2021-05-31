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
        [LabelText("是否参照对象")]
        public bool isObj = true;
        [LabelText("目标点")]
        [EnableIf("isObj")]
        public GameObject target;
        [LabelText("偏移")]
        [DisableIf("isObj")]
        public Vector3 offset;
        [LabelText("运动时间")]
        public float time;
        [LabelText("延迟时间")]
        public float delay;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if (isObj)
            {
                if (target == null)
                {
                    target = unartPara[0];
                    if (target == null)
                    {
                        DebugHelper.LogError(gameObject.name + "直线运动事件没有target参数");
                        return;
                    }
                }
            }
            if (mover == null)
                mover = gameObject;
            if (_timer != 0)
                return;
            _timer = TimerManager.Instance.AddTimer(delay, 0, 1, EventStart, null, Move);
        }

        private void Move()
        {
            if(isObj)
                mover.transform.DOMove(target.transform.position, time, false).onComplete = EventEnd;
            else
                mover.transform.DOMove(mover.transform.position+offset, time, false).onComplete = EventEnd;
        }

        public override void EventEnd()
        {
            base.EventEnd();
            _timer = 0;
        }
    }
}
