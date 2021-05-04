using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Reflection;

namespace AiLing
{
    [GameEventInfo("改变顶点位置")]
    public class PointTranslationEvent : GameEvent
    {
        [LabelText("方向向量和")]
        public Vector3 dir;
        [LabelText("偏移量")]
        public Vector3 offset;
        [LabelText("物体")]
        public GameObject obj;
        [LabelText("效果时间")]
        public int time;
        private Material _mat;
        private MeshRenderer _render;
        private BoxCollider _col;
        private float _width;
        private float _height;
        private int _timer;
        private Vector3 _colOriginCenter;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            Debug.Log("change point");
            _render = obj.GetComponent<MeshRenderer>();
            if (_render == null)
            {
                Debug.LogError(obj.name + "没有meshRender");
                return;
            }
            _width = _render.bounds.size.x;
            _height = _render.bounds.size.y;
            _col = obj.GetComponent<BoxCollider>();
            if (_col == null)
            {
                Debug.LogError(obj.name + "没有BoxCollider");
                return;
            }
            _colOriginCenter = _col.center;
            _mat = _render.material;
            _mat.SetFloat("_OriginWidth", _width);
            Debug.Log("_OriginWidth:"+_width);
            _mat.SetFloat("_OriginHeight", _height);
            Debug.Log("_OriginHeight:" + _height);
            _mat.SetVector("_Dir", dir);
            if (_timer != 0)
                TimerManager.Instance.RemoveTimer(_timer);
            _timer = TimerManager.Instance.AddTimer(time, 0, 1, EventStart, ChangeValue, EventEnd);
        }
        
        private void ChangeValue(float leftTime)
        {
            Vector3 change = offset * ((time - leftTime) / time);
            _mat.SetVector("_Offset", change);
            Debug.Log("_Offset:" + change);
            Vector3 size;
            //如果改变量和改变方向同号，则size必然增加
            if (change.x * dir.x > 0)
            {
                size.x = 1+Mathf.Abs(change.x);
            }
            else
                size.x = 1-Mathf.Abs(change.x);
            //如果改变量和改变方向同号，则size必然增加
            if (change.y * dir.y > 0)
            {
                size.y = 1 + Mathf.Abs(change.y);
            }
            else
                size.y = 1 - Mathf.Abs(change.y);
            if (change.z * dir.z > 0)
            {
                size.z = 1 + Mathf.Abs(change.z);
            }
            else
                size.z = 1 - Mathf.Abs(change.z);
            _col.center =_colOriginCenter + change/2 ;
            _col.size = size;
        }
    }
}

