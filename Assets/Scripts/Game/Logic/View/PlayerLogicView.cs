using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class PlayerLogicView : BaseView
    {
        private Animator _animator;
        private LogicContainer _container;
        private CreatureInfo _creatureInfo;
        private PlayerLifeAnimatorSetter _playerLifeAnimatorSetter;

        void Start()
        {
            _animator = GetComponent<Animator>();
            _container = GetComponent<LogicContainer>();
            _creatureInfo = _container.GetSingletonLogicCompoent<CreatureInfo>();
            _playerLifeAnimatorSetter = _container.GetSingletonLogicCompoent<PlayerLifeAnimatorSetter>();
            _playerLifeAnimatorSetter.InitAnimatorInfo();
        }

        public void OnRegenerate()
        {
            _creatureInfo.isDead = false;
            Transform regenerationTrans = transform.Find(REGENERATION_LISTENER_PATH);
            if (regenerationTrans == null)
            {
                DebugHelper.LogError("cant find deathTrans via path:" + REGENERATION_LISTENER_PATH + " on" + gameObject.name);
                return;
            }
            PassiveListener regenerationListener = regenerationTrans.GetComponent<PassiveListener>();
            if (regenerationListener == null)
            {
                DebugHelper.LogError("cant find PassiveListener via path:" + REGENERATION_LISTENER_PATH + " on" + gameObject.name);
                return;
            }
            regenerationListener.Call(unartPara);
        }

        public void OnDead()
        {
            _creatureInfo.isDead = true;
            Transform deathTrans = transform.Find(DEATH_LISTENER_PATH);
            if (deathTrans == null)
            {
                DebugHelper.LogError("cant find deathTrans via path:" + DEATH_LISTENER_PATH + " on" + gameObject.name);
                return;
            }
            PassiveListener deathListener = deathTrans.GetComponent<PassiveListener>();
            if (deathListener == null)
            {
                DebugHelper.LogError("cant find PassiveListener via path:" + DEATH_LISTENER_PATH + " on" + gameObject.name);
                return;
            }
            deathListener.Call(unartPara);
        }

        // Update is called once per frame
        void Update()
        {
            _playerLifeAnimatorSetter.SetAnimatorInfo(_creatureInfo);
        }
    }
}

