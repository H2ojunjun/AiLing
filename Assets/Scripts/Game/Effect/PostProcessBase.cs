using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessBase : MonoBehaviour
{
    protected void NotSupported()
    {
        enabled = false;
    }

    protected Material CheckShaderAndMaterial(Shader shader, Material material)
    {
        if (shader == null)
            return null;


        if (shader.isSupported && material != null && material.shader == shader)
            return material;
        else
        {
            material = new Material(shader)
            {
                hideFlags = HideFlags.DontSave
            };
            return material;
        }
    }
}
