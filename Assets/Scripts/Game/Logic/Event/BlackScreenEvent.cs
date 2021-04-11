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
        [LabelText("黑屏shader")]
        public Shader shader;

        private BlackScreenPostProcess blackPP;
        int timer;
        public override void Excute(params object[] unartPara)
        {
            base.Excute(unartPara);
            blackPP =  GameMainManager.Instance.mainCamera.gameObject.AddComponent<BlackScreenPostProcess>();
            blackPP.blackScreenShader = shader;
            if (timer != 0)
                TimerManager.Instance.RemoveTimer(timer);
            timer = TimerManager.Instance.AddTimer(blackTime, 0, 1, EventStart, FadeBlack, EventEnd);
        }

        private void FadeBlack(float time)
        {
            if (blackTime - time < fadeIn && fadeIn != 0)
                blackPP.lerp = (blackTime - time) / fadeIn;
            else if (time < fadeOut && fadeOut != 0)
                blackPP.lerp = time / fadeOut;
            else
                blackPP.lerp = 1;
        }

        public override void EventEnd()
        {
            base.EventEnd();
            Destroy(blackPP);
        }

        public override GameEventInfoAttribute GetEventAttribute()
        {
            Type t = typeof(BlackScreenEvent);
            GameEventInfoAttribute attri = t.GetCustomAttribute<GameEventInfoAttribute>();
            return attri;
        }
    }
}

