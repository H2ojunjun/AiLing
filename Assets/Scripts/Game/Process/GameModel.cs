using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AiLing
{
    [Serializable]
    public class GameModel
    {
        //存档ID
        public int id;
        //当前场景名
        public string sceneName;
        //当前小存档点
        public int section;
        //坐标
        public float x = float.MinValue;

        public float y = float.MinValue;

        public float z = float.MinValue;
    }
}

