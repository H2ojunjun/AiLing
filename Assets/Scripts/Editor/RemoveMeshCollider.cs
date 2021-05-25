using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RemoveCollider : MonoBehaviour
{
    [MenuItem("工具/删除Collider")]
    public static void RemoveMeshColliderInEditor()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (var item in objs)
        {
            foreach (var trans in item.transform)
            {
                Transform realTrans = trans as Transform;
                Collider col = realTrans.gameObject.GetComponent<Collider>();
                if (col != null)
                {
                    DestroyImmediate(col);
                }
            }
        }
    }
}
