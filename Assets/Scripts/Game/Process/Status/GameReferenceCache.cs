using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public abstract class GameReferenceCache : LogicComponent
    {
        public GameObject target;

        //缓存
        public abstract void Cache();

        //读取
        public abstract void Read();
    }
}

