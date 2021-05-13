using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class PlayerLifeAnimatorSetter : AnimatorSetter
    {
        int isDeadlID;

        public override void InitAnimatorInfo()
        {
            isDeadlID = Animator.StringToHash("isDead");
        }

        public override void SetAnimatorInfo(LogicComponent component)
        {
            if (animator == null)
            {
                DebugHelper.LogError("animator is null!");
                return;
            }
            PlayerLife playerLife = component as PlayerLife;
            animator.SetBool(isDeadlID, playerLife.isDead);
        }
    }
}

