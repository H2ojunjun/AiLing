using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("杀死事件")]
    public class MakeDeathEvent : GameEvent
    {
        [LabelText("目标(想要player死就不传)")]
        public GameObject target;

        private GameObject _realTarget;
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
            creature.OnDead();
            GameObject deathEventPrefab = _realTarget.transform.Find("events/deathEvents").gameObject;
            if (deathEventPrefab != null)
            {
                GameEvent.CallEventPrefab(deathEventPrefab,unartPara);
            }
        }
    }
}

