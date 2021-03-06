using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class MovementAnimatorSetter : AnimatorSetter
    {
        int speedHorizontalID;
        int speedVerticalID;
        int isWalkID;
        int isRunID;
        int isInAirID;
        int moveSpeedID;
        int speedHorizontalAbsID;
        int isRightID;
        int isPushID;
        int isPullID;
        int isPressID;

        public override void InitAnimatorInfo()
        {
            speedHorizontalID = Animator.StringToHash("speedHorizontal");
            speedVerticalID = Animator.StringToHash("speedVertical");
            isWalkID = Animator.StringToHash("isWalk");
            isRunID = Animator.StringToHash("isRun");
            isInAirID = Animator.StringToHash("isInAir");
            moveSpeedID = Animator.StringToHash("moveSpeed");
            speedHorizontalAbsID = Animator.StringToHash("speedHorizontalAbs");
            isRightID = Animator.StringToHash("isRight");
            isPushID = Animator.StringToHash("isPush");
            isPullID = Animator.StringToHash("isPull");
            isPressID = Animator.StringToHash("isPress");
        }

        public override void SetAnimatorInfo(LogicComponent component)
        {
            if (animator == null)
            {
                DebugHelper.LogError("animator is null!");
                return;
            }
            Movement movement = component as Movement;
            animator.SetFloat(speedHorizontalID, movement.speedHorizontal);
            animator.SetFloat(speedVerticalID, movement.speedVerticalFake);
            animator.SetBool(isWalkID, movement.isWalk);
            animator.SetBool(isRunID, movement.isRun);
            animator.SetBool(isInAirID, movement.isInAir);
            animator.SetFloat(moveSpeedID, movement.moveSpeedFake);
            animator.SetFloat(speedHorizontalAbsID, Mathf.Abs(movement.speedHorizontal));
            animator.SetBool(isRightID, movement.isRight);
            animator.SetBool(isPushID, movement.isPush);
            animator.SetBool(isPullID, movement.isPull);
            animator.SetBool(isPressID, movement.isPress);
        }
    }
}

