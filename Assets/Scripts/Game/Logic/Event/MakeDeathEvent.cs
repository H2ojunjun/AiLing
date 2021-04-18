using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [GameEventInfo("杀死事件", true)]
    public class MakeDeathEvent : GameEvent
    {
        [LabelText("目标(想要player死就不传)")]
        public GameObject target;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if (target == null)
            {
                target = unartPara[0] as GameObject;
            }
            LogicContainer container = target.GetComponent<LogicContainer>();
            if (container == null)
            {
                Debug.LogError("container is null，please check does:" + target.name + "have LogicContainer");
                return;
            }
            Creature creature = container.GetSingletonLogicCompoent<Creature>();
            if (creature == null)
            {
                Debug.LogError("creature is null，please check does creature attached on" + target.name + "'s LogicContainer");
                return;
            }
            creature.OnDead();
            GameObject deathEventPrefab = target.transform.Find("events/deathEvents").gameObject;
            if (deathEventPrefab != null)
            {
                GameEvent.CallEventPrefab(deathEventPrefab,unartPara);
            }
        }

        public override GameEventInfoAttribute GetEventAttribute()
        {
            Type t = typeof(MakeDeathEvent);
            GameEventInfoAttribute attri = t.GetCustomAttribute<GameEventInfoAttribute>();
            return attri;
        }
    }
}

