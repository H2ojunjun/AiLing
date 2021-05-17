using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class MaterialReferenceCache : GameReferenceCache
    {
        Material _mat;
        Renderer _render;
        public override void Cache()
        {
            _mat = Object.Instantiate(_render.material);
        }

        public override void Init(GameObject obj)
        {
            _render = obj.GetComponent<Renderer>();
        }

        public override void Read()
        {
            _render.material = _mat;
        }
    }
}

