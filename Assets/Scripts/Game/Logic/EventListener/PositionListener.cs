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
        public LayerMask layer = 1 << 10;
        [LabelText("碰撞目标")]
        public GameObject target;
        [LabelText("是否trigger触发")]
        public bool isTrigger = true;

        private BoxCollider _collider;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<BoxCollider>();
            if (_collider == null)
                _collider = gameObject.AddComponent<BoxCollider>();
            _collider.isTrigger = isTrigger;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isTrigger)
                return;
            GameObject obj = other.gameObject;
            if (!ShouldExcute(obj))
                return;
            unartificialPara.Clear();
            this.unartificialPara.Add(obj);
            base.CallEvent();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isTrigger)
                return;
            GameObject obj = collision.gameObject;
            if (!ShouldExcute(obj))
                return;
            unartificialPara.Clear();
            this.unartificialPara.Add(obj);
            base.CallEvent();
        }



        private bool ShouldExcute(GameObject obj)
        {
            if (target != null)
            {
                if (target != obj)
                    return false;
                else
                    return true;
            }
            else
            {
                return (((1 << obj.layer) & layer) != 0);
            }
        }
    }
}

