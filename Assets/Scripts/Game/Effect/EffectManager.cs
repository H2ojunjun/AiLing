using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class EffectManager : MonoSingleton<EffectManager>
    {
        public Camera mainCam;

        private Dictionary<Camera, List<PostProcessBase>> _postProcessDic = new Dictionary<Camera, List<PostProcessBase>>();

        private void Start()
        {
            mainCam = GameMainManager.Instance.mainCamera;
        }

        public T CreatePostProcess<T>(Shader shader,Camera cam = null) where T:PostProcessBase,new()
        {
            if (cam == null)
                cam = mainCam;
            T pp = cam.gameObject.GetComponent<T>();
            if (pp != null)
            {
                return pp;
            }
            pp = cam.gameObject.AddComponent<T>();
            if (_postProcessDic.ContainsKey(cam))
            {
                _postProcessDic[cam].Add(pp);
            }
            else
            {
                List<PostProcessBase> list = new List<PostProcessBase>();
                list.Add(pp);
                _postProcessDic.Add(cam, list);
            }
            pp.shader = shader;
            return pp;
        }

        public T CreatePostProcess<T>(Material mat, Camera cam = null) where T : PostProcessBase, new()
        {
            if (cam == null)
                cam = mainCam;
            T pp = cam.gameObject.GetComponent<T>();
            if (pp != null)
            {
                return pp;
            }
            pp = cam.gameObject.AddComponent<T>();
            if (_postProcessDic.ContainsKey(cam))
            {
                _postProcessDic[cam].Add(pp);
            }
            else
            {
                List<PostProcessBase> list = new List<PostProcessBase>();
                list.Add(pp);
                _postProcessDic.Add(cam, list);
            }
            pp.material = mat;
            return pp;
        }

        public void RemovePostProcess<T>(Camera cam =null) where T : PostProcessBase, new()
        {
            if (cam == null)
                cam = mainCam;
            T pp = cam.gameObject.GetComponent<T>();
            if (pp == null)
                return;
            _postProcessDic[cam].Remove(pp);
            Destroy(pp);
        }

        public void ChangeShader(PostProcessBase pp,Shader shader)
        {
            pp.shader = shader;
        }
    }
}

