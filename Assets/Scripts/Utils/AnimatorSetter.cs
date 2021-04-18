using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public abstract class AnimatorSetter
    {
        public Animator animator;

        public AnimatorSetter(Animator animator)
        {
            this.animator = animator;
        }

        public abstract void InitAnimatorInfo();

        public abstract void SetAnimatorInfo(LogicComponent component);
    }
}

