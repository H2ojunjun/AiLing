using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


namespace AiLing
{
    [GameEventInfo("破碎")]
    public class ShatterEvent : GameEvent
    {
        [LabelText("目标")]
        public GameObject target;
        [LabelText("是否点分割")]
        public bool isPoint;
        [LabelText("切割中心偏移量")]
        [EnableIf("isPoint")]
        public Vector3 offsetFromCenter = Vector3.zero;
        [LabelText("分割平面")]
        [DisableIf("isPoint")]
        public List<Plane> planes = new List<Plane>();
        [LabelText("爆炸力")]
        public float explosionForce;
        [LabelText("爆炸范围")]
        public float explosionRaduis;
        [LabelText("是否local坐标爆炸")]
        public bool isLocalExplode;
        [LabelText("切割后爆炸点偏移量")]
        [EnableIf("isLocalExplode")]
        public Vector3 explosionOffset;
        [LabelText("切割后爆炸中心")]
        [DisableIf("isLocalExplode")]
        public GameObject explosionCenter;
        [LabelText("延迟")]
        public float delay;
        private int _timer;
        private ShatterTool _st;
        private ShatterHanlder _sh;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            if(target == null)
            {
                target = unartPara[0];
            }
            _st = target.GetComponent<ShatterTool>();
            if(_st == null)
            {
                DebugHelper.LogError(target.name + "没有ShatterTool组件");
                return;
            }
            _sh = target.GetComponent<ShatterHanlder>();
            if (_sh == null)
            {
                DebugHelper.LogError(target.name + "没有ShatterHanlder组件");
                return;
            }
            if (delay > 0)
            {
                if (_timer == 0)
                {
                    TimerManager.Instance.AddTimer(delay, 0, 1, null, null, () =>
                    {
                        Shatter();
                        EventEnd();
                    });
                }
                else
                    return;
            }
            else
                Shatter();
        }

        private void Shatter()
        {
            if (isPoint)
                _st.Shatter(transform.position + offsetFromCenter);
            else
                _st.Split(planes.ToArray());
            _sh.postSplitCallBack = AfterSplit;
        }
        
        private void AfterSplit(GameObject[] newGameObjects)
        {
            Vector3 center;
            if (isLocalExplode)
            {
                center = target.transform.position + explosionOffset;
            }
            else
            {
                center = explosionCenter.transform.position;
            }
            foreach(GameObject item in newGameObjects)
            {
                Rigidbody body = item.GetComponent<Rigidbody>();
                body.AddExplosionForce(explosionForce,center, explosionRaduis);
            }
        }

        public override void EventEnd()
        {
            base.EventEnd();
            _timer = 0;
        }
    }
}

