using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [RequireComponent(typeof(LogicContainer),typeof(PlayerPhysicsBehaviour))]
    public class PlayerInputInstance : MonoBehaviour
    {
        private PlayerPhysicsBehaviour _physicsBehaviour;

        void Start()
        {
            _physicsBehaviour = GetComponent<PlayerPhysicsBehaviour>();
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

        private void HorizontalMove()
        {
            float horizontalInput = InputManager.Instance.GetHorizontal();
            _physicsBehaviour.horizontalInput = horizontalInput;
        }

        private void FixedUpdate()
        {
            HorizontalMove();
            Jump();
            Push();
            BreakPush();
        }
    }
}

