using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AiLing
{
    public class GameOptimizer : MonoSingleton<GameOptimizer>
    {
        private int _timer;

        public float interval;
        void Start()
        {
            _timer = TimerManager.Instance.AddTimer(interval, 0, -1, null, null, UnloadUnusedAssets);
        }

        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}

