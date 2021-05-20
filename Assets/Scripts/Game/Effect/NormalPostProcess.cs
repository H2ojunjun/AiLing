using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class NormalPostProcess : PostProcessBase
    {
        private Material normalMat
        {
            get
            {
                return CheckShaderAndMaterial();
            }
        }
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {

            if (normalMat != null)
            {
                DebugHelper.Log("OnRenderImage");
                Graphics.Blit(src, dest, normalMat);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }

}
