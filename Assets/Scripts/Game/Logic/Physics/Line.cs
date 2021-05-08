using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Transform originPoint;

    public Transform targetPoint;

    public float length { get { return Vector3.Distance(originPoint.position, targetPoint.position); } }

    public Vector3 direction { get { return (targetPoint.position - originPoint.position).normalized; ; } }
    public Vector3 center { get { return (originPoint.position + originPoint.position) / 2; } }

    public Line(Transform p1, Transform p2)
    {
        originPoint = p1;
        targetPoint = p2;
    }

    //获得某个点到改线段的垂线到起点的距离的比例，如果垂点在线段外，小于的返回0，超出的返回真正值
    public float GetVerticalIntersectionPointRatio(Vector3 point)
    {
        float t;
        t = ((point.x - originPoint.position.x) * direction.x + (point.y - originPoint.position.y) * direction.y + (point.z - originPoint.position.z) * direction.z) /
            (Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2) + Mathf.Pow(direction.z, 2));
        return Mathf.Max(0, t / length);
    }
}
