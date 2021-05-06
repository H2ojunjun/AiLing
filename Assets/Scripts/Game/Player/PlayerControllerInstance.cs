using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [RequireComponent(typeof(LogicContainer),typeof(PlayerPhysicsBehaviour))]
    public class PlayerControllerInstance : MonoBehaviour
    {
        private LogicContainer _container;
        private Movement _movement;
        private PlayerLife _playerLife;
        private MovementAnimatorSetter _movementAnimSetter;
        private PlayerLifeAnimatorSetter _playerLifeAnimatorSetter;
        private Animator _animator;
        private PlayerPhysicsBehaviour _physicsBehaviour;

        void Start()
        {
            _animator = GetComponent<Animator>();
            _physicsBehaviour = GetComponent<PlayerPhysicsBehaviour>();
            _container = GetComponent<LogicContainer>();
            _movement = _container.AddSingletonLogicComponent<Movement>();
            _playerLife = _container.AddSingletonLogicComponent<PlayerLife>();
            _movementAnimSetter = new MovementAnimatorSetter(_animator);
            _playerLifeAnimatorSetter = new PlayerLifeAnimatorSetter(_animator);
            _movementAnimSetter.InitAnimatorInfo();
            _playerLifeAnimatorSetter.InitAnimatorInfo();
        }

        private void Jump()
        {
            if (InputManager.Instance.GetKeyDown(KeyCode.JoystickButton0) || InputManager.Instance.GetKeyDown(KeyCode.Space))
                _physicsBehaviour.readyForJump = true;
        }

        private void Push()
        {
            if(InputManager.Instance.GetKeyPress(KeyCode.Mouse0) || InputManager.Instance.GetKeyPress(KeyCode.JoystickButton1))
                _physicsBehaviour.readyForPush = true;
        }

        private void BreakPush()
        {
            if (InputManager.Instance.GetKeyUp(KeyCode.Mouse0) || InputManager.Instance.GetKeyUp(KeyCode.JoystickButton1))
                _physicsBehaviour.readyForBreakPush = true;
        }

        private void FixedUpdate()
        {
            Jump();
            Push();
            BreakPush();
            _movementAnimSetter.SetAnimatorInfo(_movement);
            _playerLifeAnimatorSetter.SetAnimatorInfo(_playerLife);
        }
    }
}

