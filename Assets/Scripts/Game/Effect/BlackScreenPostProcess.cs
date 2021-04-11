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

    //public float fadeIn = 1.0f;

    //public float duration = 1.0f;

    //public float fadeOut = 1.0f;

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
        //if (fadeIn > 0)
        //    fadeIn = fadeIn - Time.deltaTime;
        //else if (duration > 0)
        //    duration = duration - Time.deltaTime;
        //else if (fadeOut > 0)
        //    fadeOut = fadeOut - Time.deltaTime;
        //BlackScreenMat.SetFloat("_FadeIn", fadeIn);
        //BlackScreenMat.SetFloat("_Duration", duration);
        //BlackScreenMat.SetFloat("_FadeOut", fadeOut);
        BlackScreenMat.SetFloat("_Lerp", lerp);
    }
}
