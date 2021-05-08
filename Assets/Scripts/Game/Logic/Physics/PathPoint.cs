using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace AiLing
{
    public class PathPoint : MonoBehaviour
    {
        [LabelText("缓动类型")]
        public EEaseType easeType;
        [HideInInspector]
        public int index;
        void Start()
        {

        }

        public Line BuildLine(PathPoint nextP)
        {
            Line line = new Line(transform,nextP.transform);
            return line;
        }

        void Update()
        {

        }
    }
}

