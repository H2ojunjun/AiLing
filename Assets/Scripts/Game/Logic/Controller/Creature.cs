using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class Creature : LogicComponent
    {
        private static List<Creature> _creatures = new List<Creature>();

        protected BaseView view;

        public bool isDead = false;

        public event Action OnDead;

        public event Action OnRegenerate;

        public override void OnCreate()
        {
            base.OnCreate();
            _creatures.Add(this);
        }

        public override void Init(GameObject obj)
        {
            this.view = obj.GetComponent<BaseView>();
            if(view == null)
            {
                DebugHelper.LogError(container.name+ "没有BaseView");
                return;
            }
        }

        public void SetUnartPara(List<GameObject> unartPara)
        {
            view.unartPara = unartPara;
        }

        //重生
        public virtual void Regenerate()
        {
            isDead = false;
            OnRegenerate?.Invoke();
        }

        public override void OnDestory()
        {
            base.OnDestory();
            _creatures.Remove(this);
        }

        public virtual void Dead()
        {
            isDead = true;
            OnDead?.Invoke();
        }
    }
}

