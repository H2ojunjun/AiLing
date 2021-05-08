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
        private Collider _collider;
        private List<PathPoint> _points = new List<PathPoint>();
        private List<Line> _lines = new List<Line>();
        private Dictionary<PlayerPhysicsBehaviour, Line> _pbLineDic = new Dictionary<PlayerPhysicsBehaviour, Line>();
        private Dictionary<Line, PathPoint> _lineStartPathPointDic = new Dictionary<Line, PathPoint>();
        private const string PATH_POINT_NAME = "pathPoint";
        private void Start()
        {
            _collider = GetComponent<Collider>();
            if(_collider == null)
            {
                DebugHelper.LogError("portal:" + gameObject.name + "没有碰撞器！");
            }
            _collider.isTrigger = true;
            for(int i = 0; i < transform.childCount; i++)
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
                _lineStartPathPointDic.Add(line, _points[i]);
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
            float ratio = first.GetVerticalIntersectionPointRatio(pb.center);
            PathPoint firstPP = _lineStartPathPointDic[first];
            Vector3 moveTarget = GetPointByRatioOnLine(firstPP,_points[firstPP.index + 1] ,ratio);
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
                float ratio = line.GetVerticalIntersectionPointRatio(pb.center);
                if (ratio > 1)
                {
                    for(int i=0;i<_lines.Count;i++)
                    {
                        if(_lines[i] == line)
                        {
                            if (i == _lines.Count - 1)
                            {
                                ExitPortal(pb);
                                return;
                            }
                            line = _lines[i + 1];
                            _pbLineDic[pb] = line;
                            break;
                        }
                    }
                }
                PathPoint pp = _lineStartPathPointDic[line];
                EEaseType ease = pp.easeType;
                switch (ease)
                {
                    case EEaseType.Liner:
                        Vector3 dir = line.direction;
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
            DebugHelper.LogError("OnTriggerExit"+other.name);
            PlayerPhysicsBehaviour pb = other.gameObject.GetComponent<PlayerPhysicsBehaviour>();
            if (pb == null)
                return;
            ExitPortal(pb);
        }

        private void ExitPortal(PlayerPhysicsBehaviour pb)
        {
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

        private Vector3 GetPointByRatioOnLine(PathPoint start,PathPoint end, float ratio)
        {
            switch (start.easeType)
            {
                case EEaseType.Liner:
                    return Vector3.Lerp(start.transform.position, end.transform.position, ratio);
                default:
                    return Vector3.zero;
            }
        }
    }
}

