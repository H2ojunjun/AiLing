using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    public class GameSceneManager01 : GameSceneManager
    {
        public ParticleSystem mainPS;

        public Vector3 mainPSOffset;

        protected override void Init()
        {
            EffectManager.Instance.FollowParticle(mainPS,GameMainManager.Instance.player, mainPSOffset);
        }

        protected override void Uninit()
        {
            if (EffectManager.Instance == null)
                return;
            EffectManager.Instance.BreakFollowParticle(mainPS);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

    }
}

