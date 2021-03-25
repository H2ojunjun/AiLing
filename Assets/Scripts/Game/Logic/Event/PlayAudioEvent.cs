using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [GameEventInfo("播放音乐", 4, new string[] { "声音枚举名:string", "次数:int", "间隔:float", "延迟:float" })]
    public class PlayAudioEvent : GameEvent
    {
        string music;
        //次数
        int time;
        //间隔
        float interval;
        float delay;
        int timer;
        public override void Excute(object[] normalPara, params object[] unartPara)
        {
            base.Excute(normalPara, unartPara);
            music = (string)normalPara[0];
            time = (int)normalPara[1];
            interval = (float)normalPara[2];

            timer = TimerManager.Instance.AddTimer(delay, interval, time, EventStart, null, () =>
            {
                AudioManager.Instance.PlaySound((EMusicName)Enum.Parse(typeof(EMusicName), music));
                EventEnd();
            });
        }
    }
}
