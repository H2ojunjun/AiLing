using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [RequireComponent(typeof(BoxCollider))]
    public class PositionListener : EventListener
    {
        [LabelText("碰撞检测层")]
        public LayerMask layer = 1<<10;

        private BoxCollider _collider;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<BoxCollider>();
            if (_collider == null)
                _collider = gameObject.AddComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            unartificialPara.Clear();
            GameObject obj = other.gameObject;
            if (((1 << obj.layer) & (1<<layer)) == 0)
                return;
            this.unartificialPara.Add(obj);
            base.CallEvent();
        }
    }
}

