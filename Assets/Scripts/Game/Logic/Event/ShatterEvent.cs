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
        [ShowIf("isPoint")]
        public Vector3 offsetFromCenter = Vector3.zero;
        [LabelText("分割平面")]
        [HideIf("isPoint")]
        public List<Plane> planes = new List<Plane>();
        [LabelText("爆炸力")]
        public float explosionForce;
        [LabelText("爆炸范围")]
        public float explosionRaduis;
        [LabelText("是否local坐标爆炸")]
        public bool isLocalExplode;
        [LabelText("切割后爆炸点偏移量")]
        [ShowIf("isLocalExplode")]
        public Vector3 explosionOffset;
        [LabelText("切割后爆炸中心")]
        [HideIf("isLocalExplode")]
        public GameObject explosionCenter;
        [LabelText("爆炸后是否应用重力")]
        public bool isUseGravity = false;
        [LabelText("爆炸后是否修改layer")]
        public bool isChangeLayer = false;
        [ShowIf("isChangeLayer")]
        [LabelText("新layer")]
        public int layer;
        [LabelText("延迟")]
        public float delay;
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
            if (_timer == 0)
            {
                _timer =TimerManager.Instance.AddTimer(delay, 0, 1, null, null, () =>
                {
                    Shatter();
                    EventEnd();
                });
            }
            else
                return;
        }

        private void Shatter()
        {
            _sh.postSplitCallBack = AfterSplit;
            if (isPoint)
                _st.Shatter(target.transform.position + offsetFromCenter);
            else
                _st.Split(planes.ToArray());
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
                body.useGravity = isUseGravity;
                if (isChangeLayer)
                {
                    item.layer = layer;
                }
            }
        }

        public override void EventEnd()
        {
            base.EventEnd();
            _timer = 0;
        }
    }
}

