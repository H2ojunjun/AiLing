using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;

namespace AiLing
{
    [GameEventInfo("改变虚拟相机跟随目标")]
    public class ChangeVirtualCameraTargetEvent : GameEvent
    {
        [LabelText("新目标")]
        public Transform newTarget;
        [LabelText("改变完成后过段时间是否改回来")]
        public bool isChangBack;
        [LabelText("时间")]
        public float changeTime;
        private Transform _oldTarget;
        private CinemachineVirtualCamera _virtualCam;
        private int _timer;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            _virtualCam = GameMainManager.Instance.virtualCam;
            if (isChangBack)
            {
                _oldTarget = _virtualCam.Follow;
                _timer = TimerManager.Instance.AddTimer(changeTime,0,1,()=> {
                    _virtualCam.Follow = newTarget;
                    EventStart();
                },null,()=> {
                    _virtualCam.Follow = _oldTarget;
                    EventEnd();
                });
            }
            else
            {
                EventStart();
                _virtualCam.Follow = newTarget;
                EventEnd();
            }
        }
    }
}

