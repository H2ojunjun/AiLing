using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class CreatureInfo : LogicComponent
    {
        private List<CreatureInfo> _allCreatureInfo = new List<CreatureInfo>();
        public bool isDead { get; set; }

        public override void OnCreate()
        {
            base.OnCreate();
            _allCreatureInfo.Add(this);
        }

        public override void Init(GameObject obj)
        {
            
        }

        public override void OnDestory()
        {
            base.OnDestory();
            _allCreatureInfo.Remove(this);
        }
    }
}

