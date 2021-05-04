using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AddMeshCollider 
{
    [MenuItem("工具/添加meshCollider")]
    public static void AddMeshColliderInEditor()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach(var item in objs)
        {
            foreach(var trans in item.transform)
            {
                Transform realTrans = trans as Transform;
                if (realTrans.gameObject.GetComponent<MeshCollider>() == null)
                {
                    realTrans.gameObject.AddComponent<MeshCollider>();
                }
            }
        }
    }
}
