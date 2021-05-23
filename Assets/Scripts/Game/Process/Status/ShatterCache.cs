using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class ShatterCache : GameReferenceCache
    {
        ShatterHanlder _sh;
        GameObject _originObj;
        public override void Cache()
        {
            
            if (_sh == null)
            {
                DebugHelper.LogError(_originObj.name + "没有ShatterHanlder");
                return;
            }
        }

        public override void Init(GameObject obj)
        {
            _sh = obj.GetComponentInChildren<ShatterHanlder>();
            _originObj = _sh.gameObject;
        }

        public override void Read()
        {
            _originObj.SetActive(true);
            Transform newTrans = _sh.newTrans;
            if (newTrans == null)
                return;
            foreach(Transform item in newTrans)
            {
                Object.Destroy(item.gameObject);
            }
            Object.Destroy(newTrans.gameObject);
        }
    }
}


