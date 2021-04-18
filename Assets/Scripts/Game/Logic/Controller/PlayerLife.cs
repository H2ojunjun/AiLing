using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class PlayerLife : Creature
    {
        public override void OnDead()
        {
            base.OnDead();
            GameMarkPointManager.Instance.GoToMark(GameProcesser.Instance.currGameModel.mark);
        }
    }
}

