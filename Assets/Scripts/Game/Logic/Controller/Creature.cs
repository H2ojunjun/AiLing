using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class Creature : LogicComponent
    {
        private static List<Creature> _creatures = new List<Creature>();

        public bool isDead = false;

        protected const string DEATH_LISTENER_PATH = "listeners/deathListener";

        public event Action OnDead;

        public event Action OnRegenerate;

        public List<GameObject> unartPara;

        public override void OnCreate()
        {
            base.OnCreate();
            _creatures.Add(this);
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

