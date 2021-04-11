using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreenPostProcess : PostProcessBase
{
    public Shader blackScreenShader;
    private Material blackScreenMat;
    public Material BlackScreenMat
    {
        get
        {
            blackScreenMat = CheckShaderAndMaterial(blackScreenShader, blackScreenMat);
            return blackScreenMat;
        }
    }

    //当前颜色值到0，0，0的插值
    public float lerp = 0;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (BlackScreenMat != null)
        {
            Graphics.Blit(src, dest, BlackScreenMat);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    private void Update()
    {
        BlackScreenMat.SetFloat("_Lerp", lerp);
    }
}
