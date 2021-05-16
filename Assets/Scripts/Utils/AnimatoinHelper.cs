using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatoinHelper
{
    public static void AddAnimationEvent(Animator animator, string clipName, string eventFunctionName, float time)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == clipName)
            {
                AnimationEvent _event = new AnimationEvent();
                _event.functionName = eventFunctionName;
                if (time == -1)
                    time = clips[i].length;
                _event.time = time;
                clips[i].AddEvent(_event);
                break;
            }
        }
        animator.Rebind();
    }
}
