using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class PlayerLife : Creature
    {
        public override void Dead()
        {
            base.Dead();
            GameMarkPointManager.Instance.GoToMark(GameProcesser.Instance.currGameModel.mark);
            Transform deathTrans = container.transform.Find(DEATH_LISTENER_PATH);
            if(deathTrans == null)
            {
                DebugHelper.LogError("cant find deathTrans via path:" + DEATH_LISTENER_PATH+" on"+container.gameObject.name);
                return;
            }
            PassiveListener deathListener = deathTrans.GetComponent<PassiveListener>();
            if (deathListener == null)
            {
                DebugHelper.LogError("cant find PassiveListener via path:" + DEATH_LISTENER_PATH + " on" + container.gameObject.name);
                return;
            }
            deathListener.Call(unartPara);
        }
    }
}

