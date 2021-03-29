using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AiLing
{
    [GameEventInfo("直线运动", 1, new string[] { "移动总时间:float" }, 1, new string[] { "目的地参照物" })]
    public class LinerMovementEvent : GameEvent
    {
    }

}
