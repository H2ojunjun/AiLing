using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("改变材质属性")]
    public class ChangeMaterialFloatValueEvent : GameEvent
    {
        [LabelText("渲染对象")]
        public Renderer render;
        [LabelText("总时间")]
        public float time;
        [LabelText("属性名")]
        public string propertyName;
        [LabelText("开始值")]
        public float start;
        [LabelText("结束值")]
        public float end;
        private int _timer;
        private Material _mat;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if(render == null)
            {
                render = unartPara[0].GetComponent<Renderer>();
                if(render == null)
                {
                    DebugHelper.LogError(gameObject.name+"上的事件没有绑定Render!");
                    return;
                }
            }
            _mat = render.material;
            _timer = TimerManager.Instance.AddTimer(time, 0, 1, EventStart, ChangeValue, EventEnd);
        }

        private void ChangeValue(float leftTime)
        {
            _mat.SetFloat(propertyName, Mathf.Lerp(start, end, (time - leftTime) / time));
        }

        public override void EventEnd()
        {
            base.EventEnd();
            _timer = 0;
        }
    }
}

