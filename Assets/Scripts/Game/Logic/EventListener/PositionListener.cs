using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [RequireComponent(typeof(BoxCollider))]
    public class PositionListener : EventListener
    {
        private BoxCollider _collider;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            unartificialPara.Clear();
            this.unartificialPara.Add(other.gameObject);
            base.CallEvent();
        }
    }
}

