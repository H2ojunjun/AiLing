using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class PlayerLife : Creature
    {
        private PlayerLogicView _realView;

        public override void Init(GameObject obj)
        {
            base.Init(obj);
            _realView = view as PlayerLogicView;
        }

        public override void Dead()
        {
            base.Dead();
            _realView.OnDead();
        }

        public override void Regenerate()
        {
            base.Regenerate();
            GameMarkPointManager.Instance.GoToMark(GameProcesser.Instance.GetCurrMark());
            foreach(var item in StatusInfo.allStatusInfo)
            {
                item.StartReference();
            }
            _realView.OnRegenerate();
        }
    }
}

