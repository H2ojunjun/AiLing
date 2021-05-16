using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public abstract class AnimatorSetter : LogicComponent
    {
        public Animator animator;

        public override void Init(GameObject obj)
        {
            animator = obj.GetComponent<Animator>();
        }

        public abstract void InitAnimatorInfo();

        public abstract void SetAnimatorInfo(LogicComponent component);
    }
}

