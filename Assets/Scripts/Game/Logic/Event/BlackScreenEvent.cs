using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AiLing
{
    [GameEventInfo("黑屏")]
    public class BlackScreenEvent : GameEvent
    {
        [LabelText("效果总时间")]
        public float blackTime;
        [LabelText("淡入时间")]
        public float fadeIn;
        [LabelText("淡出时间")]
        public float fadeOut;
        [LabelText("灯光")]
        public Light blackLight;
        int timer;
        public override void Excute(params object[] unartPara)
        {
            base.Excute(unartPara);
            if (timer != 0)
                TimerManager.Instance.RemoveTimer(timer);
            timer = TimerManager.Instance.AddTimer(blackTime, 0, 1, EventStart, FadeBlack, EventEnd);
        }

        private void FadeBlack(float time)
        {
            if (blackTime - time < fadeIn && fadeIn != 0)
                blackLight.intensity = 1 - (blackTime - time) / fadeIn;
            else if (time < fadeOut && fadeOut != 0)
                blackLight.intensity = 1 - time / fadeOut;
            else
                blackLight.intensity = 0;
        }

        public override GameEventInfoAttribute GetEventAttribute()
        {
            Type t = typeof(BlackScreenEvent);
            GameEventInfoAttribute attri = t.GetCustomAttribute<GameEventInfoAttribute>();
            return attri;
        }
    }
}

