using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("死亡")]
    public class MakeDeathEvent : GameEvent
    {
        private GameObject _realTarget;

        [InfoBox("想要player死就不传")]
        [LabelText("目标")]
        public GameObject target;
        [LabelText("延迟时间")]
        public float time;

        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if (target == null)
            {
                _realTarget = unartPara[0];
            }
            else
                _realTarget = target;
            LogicContainer container = _realTarget.GetComponent<LogicContainer>();
            if (container == null)
            {
                return;
            }
            Creature creature = container.GetSingletonLogicCompoent<Creature>();
            if (creature == null)
            {
                return;
            }
            if (_timer != 0)
                return;
            _timer = TimerManager.Instance.AddTimer(time, 0, 1, null, null, () => {
                creature.SetUnartPara(unartPara);
                creature.Dead();
                _timer = 0;
            });
        }
    }
}

