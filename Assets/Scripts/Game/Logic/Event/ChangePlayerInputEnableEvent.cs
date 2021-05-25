using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("屏蔽/开启玩家输入")]
    public class ChangePlayerInputEnableEvent : GameEvent
    {
        [LabelText("目标")]
        public PlayerInputInstance target;
        [LabelText("是否屏蔽")]
        public bool isMute;
        [LabelText("是否恢复")]
        public bool isBack;
        [EnableIf("isBack")]
        [LabelText("恢复时间")]
        public float time;
        [LabelText("延迟时间")]
        public float delay;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if (target == null)
                target = GameMainManager.Instance.mainPlayerInput;
            if (isBack)
            {
                if (_timer != 0)
                    TimerManager.Instance.RemoveTimer(_timer);
                _timer = TimerManager.Instance.AddTimer(time, delay, 1,()=> {
                    EventStart();
                    ChangeEnable();
                }, null, ()=> {
                    RevertEnable();
                    EventEnd();
                });
            }
            else
            {
                if (_timer != 0)
                    TimerManager.Instance.RemoveTimer(_timer);
                _timer = TimerManager.Instance.AddTimer(delay, 0, 1,null, null, () => {
                    EventStart();
                    ChangeEnable();
                    EventEnd();
                });
            }
        }

        private void ChangeEnable()
        {
            target.enabled = !isMute;
        }

        private void RevertEnable()
        {
            target.enabled = isMute;
        }
    }
}


