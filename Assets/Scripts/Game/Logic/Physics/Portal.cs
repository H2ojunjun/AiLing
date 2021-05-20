using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace AiLing
{
    public class Portal : MonoBehaviour
    {
        [LabelText("重力大小")]
        public float gravity = 10;
        [LabelText("重力场水平速度限制")]
        public float horizontalSpeedLimit = 4;
        [LabelText("进入引力场时间")]
        public float gravityFieldDuration = 2;
        [LabelText("下一个连接的portal")]
        public Portal nextConnectPortal;
        [LabelText("离开后是否自动反向")]
        public bool autoChangeDirection;
        public bool isPositive 
        {
            get 
            {
                return _isPositive; 
            } 
            set 
            {
                ChangeDirection(value);
            } 
        }
        private bool _isPositive = true;
        private Collider _collider;
        private List<PathPoint> _points = new List<PathPoint>();
        private List<Line> _lines = new List<Line>();
        private Dictionary<PlayerPhysicsBehaviour, Line> _pbLineDic = new Dictionary<PlayerPhysicsBehaviour, Line>();
        private const string PATH_POINT_NAME = "pathPoint";
        private Material _mat;
        private void Start()
        {
            _collider = GetComponent<Collider>();
            if(_collider == null)
            {
                DebugHelper.LogError("portal:" + gameObject.name + "没有碰撞器！");
            }
            _collider.isTrigger = true;
            _mat = GetComponent<Renderer>().material;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform trans = transform.GetChild(i);
                if (trans.name.Contains(PATH_POINT_NAME))
                {
                    PathPoint pp = trans.GetComponent<PathPoint>();
                    if(pp != null)
                    {
                        pp.index = i;
                        _points.Add(pp);
                    }
                }
            }
            if (_points.Count < 2)
            {
                DebugHelper.LogError("portal:" + transform.name + "的路径点数量小于2！");
            }
            for(int i = 0; i < _points.Count-1; i++)
            {
                Line line = _points[i].BuildLine(_points[i + 1]);
                _lines.Add(line);
                line.index = i;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            PlayerPhysicsBehaviour pb = other.gameObject.GetComponent<PlayerPhysicsBehaviour>();
            if (pb == null)
                return;
            if (_pbLineDic.ContainsKey(pb))
                return;
            pb.SetRealHorizontalSpeed(horizontalSpeedLimit);
            pb.SetRealGravity(gravity);
            Line first = GetNearestLine(pb.center);
            _pbLineDic.Add(pb, first);
            float ratio = first.GetVerticalIntersectionPointRatio(pb.center,_isPositive);
            PathPoint firstPP = _points[first.index];
            PathPoint secondPP = _points[first.index+1];
            Vector3 moveTarget = GetPointByRatioOnLine(firstPP, secondPP, ratio,_isPositive);
            pb.moveTowardsPara = new MoveTowardsParameter(moveTarget, new Vector3(0, 0, 0), 0, gravityFieldDuration);
            DebugHelper.Log("moveTarget"+ moveTarget);
            pb.readyForMoveTowardsEndInSpeed = true;
            pb.canMove = false;
        }

        private void OnTriggerStay(Collider other)
        {
            PlayerPhysicsBehaviour pb = other.gameObject.GetComponent<PlayerPhysicsBehaviour>();
            if (pb == null)
                return;
            if (pb.moveTowardsPara != null)
                return;
            Line line;
            if(_pbLineDic.TryGetValue(pb,out line))
            {
                float ratio = line.GetVerticalIntersectionPointRatio(pb.center,_isPositive);
                if (ratio > 1)
                {
                    if (_isPositive)
                    {
                        if(line.index == _lines.Count - 1)
                        {
                            if (autoChangeDirection)
                                ChangeDirection(!_isPositive);
                            ExitPortal(pb);
                            return;
                        }
                        line = _lines[line.index + 1];
                        _pbLineDic[pb] = line;
                    }
                    else
                    {
                        if (line.index == 0)
                        {
                            if (autoChangeDirection)
                                ChangeDirection(!_isPositive);
                            ExitPortal(pb);
                            return;
                        }
                        line = _lines[line.index-1];
                        _pbLineDic[pb] = line;
                    }
                }
                PathPoint pp = _points[line.index];
                EEaseType ease = pp.easeType;
                switch (ease)
                {
                    case EEaseType.Liner:
                        Vector3 dir;
                        if (_isPositive)
                            dir = line.direction;
                        else
                            dir = line.inverseDirection;
                        pb.SetRealGravityVec(dir);
                        DebugHelper.Log("dir" + dir);
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerPhysicsBehaviour pb = other.gameObject.GetComponent<PlayerPhysicsBehaviour>();
            if (pb == null)
                return;
            ExitPortal(pb);
        }

        private void ExitPortal(PlayerPhysicsBehaviour pb)
        {
            if(_pbLineDic.ContainsKey(pb))
                _pbLineDic.Remove(pb);
            if (nextConnectPortal != null)
                return;
            pb.ResetRealGravity();
            pb.ReSetRealGravityVec();
            pb.ResetRealHorizontalSpeed();
            pb.canMove = true;
        }

        private Line GetNearestLine(Vector3 point)
        {
            float dis = float.MaxValue;
            Line result = null;
            foreach(var line in _lines)
            {
                float distance = Vector3.Distance(line.center, point);
                if (distance < dis)
                {
                    result = line;
                    dis = distance;
                }
            }
            return result;
        }

        private Vector3 GetPointByRatioOnLine(PathPoint start,PathPoint end, float ratio,bool isPositive)
        {
            switch (start.easeType)
            {
                case EEaseType.Liner:
                    if(isPositive)
                        return Vector3.Lerp(start.transform.position, end.transform.position, ratio);
                    else
                        return Vector3.Lerp(end.transform.position, start.transform.position, ratio);
                default:
                    return Vector3.zero;
            }
        }

        public void ChangeDirection(bool isPositive)
        {
            if (isPositive == _isPositive)
                return;
            Vector4 orginUVVec = _mat.GetVector("_Direction");
            _mat.SetVector("_Direction", -orginUVVec);
            _isPositive = isPositive;
        }
    }
}

