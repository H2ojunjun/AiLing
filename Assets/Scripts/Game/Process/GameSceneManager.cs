using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public abstract class GameSceneManager : MonoSingleton<GameSceneManager>
    {
        protected virtual void Start()
        {
            Init();
        }

        protected abstract void Init();


        protected abstract void Uninit();

        protected virtual void OnDestroy()
        {
            Uninit();
        }
    }
}

