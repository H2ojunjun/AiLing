using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessBase : MonoBehaviour
{
    public Shader shader;
    public Material material;

    protected Material CheckShaderAndMaterial()
    {
        if (material != null)
            return material;

        if (shader != null && shader.isSupported)
        {
            material = new Material(shader)
            {
                hideFlags = HideFlags.DontSave
            };
            return material;
        }
        return null;
    }
}
