using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [GameEventInfo("黑屏",3,new string[] {"效果总时间:float","淡入时间:float", "淡出时间:float" },1,new string[] { "灯光"})]
    public class BlackScreenEvent : GameEvent
    {
        float blackTime;
        float fadeIn;
        float fadeOut;
        Light light;
        int timer;
        public override void Excute(object[] normalPara, params object[] unartPara)
        {
            base.Excute(normalPara,unartPara);
            blackTime = (float)normalPara[0];
            fadeIn = (float)normalPara[1];
            fadeOut = (float)normalPara[2];
            GameObject obj = (GameObject)normalPara[3];
            light = obj.GetComponent<Light>();
            if (timer != 0)
                TimerManager.Instance.RemoveTimer(timer);
            timer = TimerManager.Instance.AddTimer(blackTime, 0, 1, null, FadeBlack, EventEnd);
        }

        private void FadeBlack(float time)
        {
            if (blackTime - time < fadeIn)
                light.intensity = 1 - (blackTime - time) / fadeIn;
            else if (time < fadeOut)
                light.intensity = 1 - time / fadeOut;
            else
                light.intensity = 0;
        }
    }
}

