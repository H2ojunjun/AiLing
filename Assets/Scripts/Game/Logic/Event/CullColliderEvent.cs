using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("开启/关闭碰撞")]
    public class CullColliderEvent:GameEvent
    {
        [LabelText("目标")]
        public Collider target;
        [LabelText("延迟时间")]
        public float time;
        [LabelText("是否开启")]
        public bool isOpen = false;
        private int _timer;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if(target == null)
            {
                target = unartPara[0].GetComponent<Collider>();
                if (target == null)
                    return;
            }
            _timer = TimerManager.Instance.AddTimer(time, 0, 1, EventStart, null,()=> {
                target.enabled = isOpen;
                _timer = 0;
                EventEnd();
            });
        }
    }
}
