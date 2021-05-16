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
            CreatureInfo creatureInfo = component as CreatureInfo;
            animator.SetBool(isDeadlID, creatureInfo.isDead);
        }
    }
}

