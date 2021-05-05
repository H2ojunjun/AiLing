using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    public class ButtonListener : EventListener
    {
        [LabelText("键")]
        public KeyCode keyName;
        [LabelText("距离")]
        public float distance;
        [LabelText("目标")]
        public GameObject target;

        private void Update()
        {
            if (InputManager.Instance.GetKeyDown(keyName) && Vector3.Distance(gameObject.transform.position, target.transform.position) <= distance)
            {
                base.CallEvent();
            }
        }
    }
}

