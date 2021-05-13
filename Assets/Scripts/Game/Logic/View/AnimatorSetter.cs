using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public abstract class AnimatorSetter : LogicComponent
    {
        public Animator animator;

        public abstract void InitAnimatorInfo();

        public abstract void SetAnimatorInfo(LogicComponent component);
    }
}

