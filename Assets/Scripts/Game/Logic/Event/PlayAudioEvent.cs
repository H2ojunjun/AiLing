using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;

namespace AiLing
{
    [GameEventInfo("播放音乐")]
    public class PlayAudioEvent : GameEvent
    {
        [LabelText("声音枚举名")]
        public EMusicName music;
        [LabelText("次数")]
        public int time = 0;
        [LabelText("间隔")]
        public float interval = 0;
        [LabelText("延迟")]
        public float delay = 0;
        int timer;
        public override void Excute(params object[] unartPara)
        {
            timer = TimerManager.Instance.AddTimer(delay, interval, time, EventStart, null, () =>
            {
                AudioManager.Instance.PlaySound(music);
                EventEnd();
            });
        }

        public override GameEventInfoAttribute GetEventAttribute()
        {
            Type t = typeof(PlayAudioEvent);
            GameEventInfoAttribute attri = t.GetCustomAttribute<GameEventInfoAttribute>();
            return attri;
        }
    }
}
