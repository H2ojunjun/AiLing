using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("剔除碰撞",true)]
    public class CullColliderEvent:GameEvent
    {
        [LabelText("目标")]
        public Collider target;
        [LabelText("延迟时间")]
        public float time;

        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if(target == null)
            {
                target = unartPara[0].GetComponent<Collider>();
                if (target == null)
                    return;
            }
            target.enabled = false;
        }
    }
}
