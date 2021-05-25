using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class SetActiveCache : GameReferenceCache
    {
        private bool isActive;
        private GameObject obj;
        public override void Cache()
        {
            isActive = obj.activeInHierarchy;
        }

        public override void Init(GameObject obj)
        {
            this.obj = obj;
        }

        public override void Read()
        {
            container.gameObject.SetActive(isActive);
        }
    }
}

