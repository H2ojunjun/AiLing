using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("展示提示")]
    public class ShowTipsUIEvent : GameEvent
    {
        [LabelText("提示内容")]
        public string content;
        [LabelText("提示颜色")]
        public Color color = Color.black;
        [LabelText("持续时间")]
        public float persistTime;
        [LabelText("延迟时间")]
        public float delay;
        UITips tip;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if (_timer != 0)
                return;
            _timer = TimerManager.Instance.AddTimer(persistTime, delay, 1, EventStart, null, EventEnd);
        }

        public override void EventStart()
        {
            base.EventStart();
            tip = UIManager.Instance.CreateNewUI<UITips>();
            tip.color = color;
            tip.content = content;
            UIManager.Instance.InitUI(tip);
            UIManager.Instance.ShowUI(tip);
        }

        public override void EventEnd()
        {
            base.EventEnd();
            UIManager.Instance.HideUI(tip);
            _timer = 0;
        }
    }
}

