using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPostProcess : PostProcessBase
{
    private Material _BlackScreenMat
    {
        get
        {
            return CheckShaderAndMaterial();
        }
    }

    //当前颜色值到0，0，0的插值
    public float lerp = 0;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_BlackScreenMat != null)
        {
            Graphics.Blit(src, dest, _BlackScreenMat);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    private void Update()
    {
        _BlackScreenMat.SetFloat("_Lerp", lerp);
    }
}
