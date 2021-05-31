using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("设置active")]
    public class SetActiveEvent : GameEvent
    {
        [LabelText("目标")]
        public GameObject target;
        public bool isActive;
        [LabelText("延迟")]
        public float time;

        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if(target == null)
            {
                target = unartPara[0];
            }
            if (_timer != 0)
                return;
            _timer = TimerManager.Instance.AddTimer(time, 0, 1, null, null, () =>
            {
                EventStart();
                target.SetActive(isActive);
                EventEnd();
                _timer = 0;
            });

        }
    }
}

