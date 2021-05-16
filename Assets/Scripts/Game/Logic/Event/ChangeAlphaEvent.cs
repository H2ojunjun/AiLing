using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("改变透明度")]
    public class ChangeAlphaEvent : GameEvent
    {
        [LabelText("目标")]
        public GameObject target;
        [LabelText("时间")]
        public float time;
        [Range(0,1)]
        [LabelText("起始值")]
        public float start;
        [Range(0, 1)]
        [LabelText("结束值")]
        public float end;
        private int _timer;
        private Renderer _render;
        private bool _hasClone =false;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if(target == null)
            {
                target = unartPara[0];
            }
            _render = target.GetComponent<Renderer>();
            if (_render == null)
                return;
            _timer = TimerManager.Instance.AddTimer(time, 0, 1, EventStart, ChangeAlpha, EventEnd);
        }

        private void ChangeAlpha(float leftTime)
        {
            Color col = _render.material.color;
            col.a = Mathf.Lerp(start, end, (time - leftTime) / time);
            if (!_hasClone)
            {
                _render.material.color = col;
                _hasClone = true;
            }
            else
                _render.sharedMaterial.color = col;
        }

        public override void EventEnd()
        {
            base.EventEnd();
            _timer = 0;
        }
    }
}

