using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GameEnumAttribute : Attribute
    {
        public string CNName;
        public GameEnumAttribute(string CNName)
        {
            this.CNName = CNName;
        }
    }

    public enum ListenerType
    {
        [GameEnum("位置监听器")]
        POS = 1
    }
}

