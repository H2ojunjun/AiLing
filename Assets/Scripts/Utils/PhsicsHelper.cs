using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public static class PhsicsHelper
    {
        public static void TransPort(Transform from,Transform to)
        {
            PlayerController.Instance.cc.enabled = false;
            from.position = to.position;
            PlayerController.Instance.cc.enabled = true;
        }
    }
}

