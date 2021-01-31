using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    public class PositionListener : EventListener
    {
        private void OnTriggerEnter(Collider other)
        {
            this.unartificialPara.Add(other.gameObject.name);
            base.CallEvent();
        }
    }
}

