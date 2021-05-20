using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class TransformReferenceCache : GameReferenceCache
    {
        Vector3 _localPosition;
        Quaternion _localRotation;
        Vector3 _localSacle;
        GameObject _obj;

        public override void Cache()
        {
            _localPosition = _obj.transform.localPosition;
            _localRotation = _obj.transform.localRotation;
            _localSacle = _obj.transform.localScale;
        }

        public override void Init(GameObject obj)
        {
            _obj = obj;
        }

        public override void Read()
        {
            container.transform.localPosition = _localPosition;
            container.transform.localRotation = _localRotation;
            container.transform.localScale = _localSacle;
        }
    }
}

