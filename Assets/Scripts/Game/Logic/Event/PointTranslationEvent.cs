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
        [LabelText("物体")]
        public GameObject obj;
        [LabelText("方向向量和")]
        public Vector3 dir;
        [LabelText("偏移量")]
        public Vector3 offset;
        [LabelText("效果时间")]
        public int time;
        private Material _mat;
        private MeshRenderer _render;
        private BoxCollider _col;
        private float _width;
        private float _height;
        private int _timer;
        private Vector3 _colOriginCenter;
        private Vector3 _colOriginSize;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            DebugHelper.Log("change point");
            _render = obj.GetComponent<MeshRenderer>();
            if (_render == null)
            {
                DebugHelper.LogError(obj.name + "没有meshRender");
                return;
            }
            _width = _render.bounds.size.x;
            _height = _render.bounds.size.y;
            _col = obj.GetComponent<BoxCollider>();
            if (_col == null)
            {
                DebugHelper.LogError(obj.name + "没有BoxCollider");
                return;
            }
            _colOriginSize = _col.size;
            _colOriginCenter = _col.center;
            _mat = _render.material;
            _mat.SetFloat("_OriginWidth", _width);
            _mat.SetFloat("_OriginHeight", _height);
            _mat.SetVector("_Dir", dir);
            if (_timer != 0)
                TimerManager.Instance.RemoveTimer(_timer);
            _timer = TimerManager.Instance.AddTimer(time, 0, 1, EventStart, ChangeValue, EventEnd);
        }
        
        private void ChangeValue(float leftTime)
        {
            Vector3 change = offset * ((time - leftTime) / time);
            _mat.SetVector("_Offset", change);
            DebugHelper.Log("_Offset:" + change);
            Vector3 size;
            //如果改变量和改变方向同号，则size必然增加
            if (change.x * dir.x > 0)
            {
                size.x = _colOriginSize.x*(1 + Mathf.Abs(change.x));
            }
            else
                size.x = _colOriginSize.x*(1 - Mathf.Abs(change.x));
            //如果改变量和改变方向同号，则size必然增加
            if (change.y * dir.y > 0)
            {
                size.y = _colOriginSize.y*(1 + Mathf.Abs(change.y));
            }
            else
                size.y = _colOriginSize.y*(1 - Mathf.Abs(change.y));
            if (change.z * dir.z > 0)
            {
                size.z = _colOriginSize.z*(1 + Mathf.Abs(change.z));
            }
            else
                size.z = _colOriginSize.y*(1 - Mathf.Abs(change.z));
            _col.center =_colOriginCenter + change/2 ;
            _col.size = size;
        }
    }
}

