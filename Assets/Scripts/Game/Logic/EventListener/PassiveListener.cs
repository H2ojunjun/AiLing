using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class PassiveListener : EventListener
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public void Call(List<GameObject> unartPara)
        {
            this.unartificialPara = unartPara;
            base.CallEvent();
        }
    }
}

