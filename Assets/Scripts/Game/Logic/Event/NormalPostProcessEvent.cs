using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("普通后处理事件")]
    public class NormalPostProcessEvent : GameEvent
    {
        [LabelText("效果总时间")]
        [InfoBox("一直出现就传-1")]
        public float totalTime;
        [LabelText("shader")]
        public Shader shader;
        [LabelText("材质")]
        public Material mat;
        [LabelText("延迟")]
        public float delay;
        int _timer;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if (_timer != 0)
                return;
            if(totalTime == -1)
            {
                _timer = TimerManager.Instance.AddTimer(delay, 0, 1, EventStart, null, () =>
                {
                    EventEnd();
                    if(mat != null)
                        EffectManager.Instance.CreatePostProcess<NormalPostProcess>(mat);
                    else
                        EffectManager.Instance.CreatePostProcess<NormalPostProcess>(shader);
                });
            }
            else
            {
                _timer = TimerManager.Instance.AddTimer(totalTime, delay, 1, ()=> {
                    EventStart();
                    if (mat != null)
                        EffectManager.Instance.CreatePostProcess<NormalPostProcess>(mat);
                    else
                        EffectManager.Instance.CreatePostProcess<NormalPostProcess>(shader);
                }, null, () =>
                {
                    EventEnd();
                    EffectManager.Instance.RemovePostProcess<NormalPostProcess>();
                });
            }
        }

        public override void EventEnd()
        {
            base.EventEnd();
            _timer = 0;
        }
    }

}

