using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class Creature : LogicComponent
    {
        private static List<Creature> _creatures = new List<Creature>();

        public bool isDead = false;
        public override void OnCreate()
        {
            base.OnCreate();
            _creatures.Add(this);
        }

        public override void OnDestory()
        {
            base.OnDestory();
            _creatures.Remove(this);
        }

        public virtual void OnDead()
        {
            isDead = true;
        }
    }
}

