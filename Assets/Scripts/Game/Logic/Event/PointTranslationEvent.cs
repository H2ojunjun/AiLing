using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Reflection;

namespace AiLing
{
    [GameEventInfo("扩散")]
    public class PointTranslationEvent : GameEvent
    {
        [LabelText("物体")]
        public GameObject obj;
        [LabelText("方向向量和")]
        [OnValueChanged("ChangeDir")]
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
        private float _depth;
        private int _timer;
        private Vector3 _originScale;
        private Vector3 _originPos;
        private Vector2 _originMainTexScale;
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            _originScale = obj.transform.localScale;
            _originPos = obj.transform.localPosition;
            _render = obj.GetComponent<MeshRenderer>();
            if (_render == null)
            {
                DebugHelper.LogError(obj.name + "没有meshRender");
                return;
            }
            _width = _render.bounds.size.x;
            _height = _render.bounds.size.y;
            _depth = _render.bounds.size.z;
            _col = obj.GetComponent<BoxCollider>();
            if (_col == null)
            {
                DebugHelper.LogError(obj.name + "没有BoxCollider");
                return;
            }
            _mat = _render.material;
            _originMainTexScale = _mat.mainTextureScale;
            if (_timer != 0)
                TimerManager.Instance.RemoveTimer(_timer);
            _timer = TimerManager.Instance.AddTimer(time, 0, 1, EventStart, ChangeValue, EventEnd);
        }
        
        private void ChangeValue(float leftTime)
        {
            Vector3 change = offset * ((time - leftTime) / time);
            Vector3 realScale;
            realScale.x = _originScale.x * (1 + change.x);
            realScale.y = _originScale.y * (1 + change.y);
            realScale.z = _originScale.z * (1 + change.z);
            obj.transform.localScale = realScale;
            Vector3 realPos;
            realPos.x = _originPos.x + (_width * change.x * dir.x)/2;
            realPos.y = _originPos.y + (_height * change.y * dir.y) / 2;
            realPos.z = _originPos.z + (_depth * change.z * dir.z) / 2;
            obj.transform.localPosition = realPos;
            Vector2 uvScale;
            uvScale.x = _originMainTexScale.x * (1 + change.x);
            uvScale.y = _originMainTexScale.y * (1 + change.y);
            _mat.mainTextureScale = uvScale;
        }

        public override void EventEnd()
        {
            base.EventEnd();
            _timer = 0;
        }

        private void ChangeDir()
        {
            Vector3 realDir = Vector3.zero;
            if (dir.x != 0)
            {
                if (dir.x > 0)
                    realDir.x = 1;
                else
                    realDir.x = -1;
            }
            if (dir.y != 0)
            {
                if (dir.y > 0)
                    realDir.y = 1;
                else
                    realDir.y = -1;
            }
            if (dir.z != 0)
            {
                if (dir.z > 0)
                    realDir.z = 1;
                else
                    realDir.z = -1;
            }
            dir = realDir;
        }
    }
}

