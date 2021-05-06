using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public static class PhsicsHelper
    {
        public static void TransPort(Transform from,Transform to)
        {
            CharacterController cc = from.GetComponent<CharacterController>();
            if (cc != null)
                cc.enabled = false;
            from.position = to.position;
            if (cc != null)
                cc.enabled = true;
        }
    }
}

