using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class ButtonListener : EventListener
    {
        public KeyCode keyName;

        public float distance;

        public GameObject target;

        private void Update()
        {
            if (Input.GetKeyUp(keyName) && Vector3.Distance(gameObject.transform.position, target.transform.position) <= distance)
            {
                CallEvent();
            }
        }
    }
}

