using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class MovementAnimatorSetter
    {
        Animator animator;
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

        public MovementAnimatorSetter(Animator animator)
        {
            this.animator = animator;
        }

        public void InitMovementAnimatorInfo()
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
        }

        public void SetMovementAnimatorInfo(Movement movement)
        {
            if (animator == null)
            {
                Debug.LogError("animator is null!");
                return;
            }
            animator.SetFloat(speedHorizontalID, movement.speedHorizontal);
            animator.SetFloat(speedVerticalID, movement.speedVerticalFake);
            animator.SetBool(isWalkID, movement.isWalk);
            animator.SetBool(isRunID, movement.isRun);
            animator.SetBool(isInAirID, movement.isInAir);
            animator.SetFloat(moveSpeedID, movement.moveSpeedFake);
            animator.SetFloat(speedHorizontalAbsID,Mathf.Abs(movement.speedHorizontal));
            animator.SetBool(isRightID, movement.isRight);
            animator.SetBool(isPushID, movement.isPush);
            animator.SetBool(isPullID, movement.isPull);
        }
    }
}

