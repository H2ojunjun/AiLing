using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class KeyStatus
    {
        public bool down;
        public bool up;
        public bool press;
        public int delayDownTime;
        public int delayUpTime;
        public int delayPressTime;

        public void CheckSelf()
        {
            if (delayDownTime == 0)
                down = false;
            if (delayPressTime == 0)
                press = false;
            if (delayUpTime == 0)
                up = false;
        }

        public void Reduce()
        {
            if (delayDownTime > 0)
                delayDownTime--;
            if (delayPressTime > 0)
                delayPressTime--;
            if (delayUpTime > 0)
                delayUpTime--;
        }
    }

    public class InputManager : MonoSingleton<InputManager>
    {
        public List<KeyCode> listenKeys = new List<KeyCode>();

        private Dictionary<KeyCode, KeyStatus> _keyStatus = new Dictionary<KeyCode, KeyStatus>();

        private void Awake()
        {
            foreach (var key in listenKeys)
            {
                _keyStatus.Add(key, new KeyStatus());
            }
        }

        public bool GetKeyDown(KeyCode key)
        {
            KeyStatus ks;
            if (_keyStatus.TryGetValue(key, out ks))
            {
                return ks.down;
            }
            return false;
        }

        public bool GetKeyUp(KeyCode key)
        {
            KeyStatus ks;
            if (_keyStatus.TryGetValue(key, out ks))
            {
                return ks.up;
            }
            return false;
        }

        public bool GetKeyPress(KeyCode key)
        {
            KeyStatus ks;
            if (_keyStatus.TryGetValue(key, out ks))
            {
                return ks.press;
            }
            return false;
        }

        public float GetHorizontal()
        {
            return Input.GetAxis("Horizontal");
        }

        public float GetVertical()
        {
            return Input.GetAxis("Vertical");
        }

        private void Update()
        {
            foreach (var item in _keyStatus)
            {
                if (Input.GetKeyDown(item.Key))
                {
                    item.Value.down = true;
                    item.Value.delayDownTime = 1;
                }
                if (Input.GetKey(item.Key))
                {
                    item.Value.press = true;
                    item.Value.delayPressTime = 1;
                }
                if (Input.GetKeyUp(item.Key))
                {
                    item.Value.up = true;
                    item.Value.delayUpTime = 1;
                }
            }
        }

        private void FixedUpdate()
        {
            foreach (var item in _keyStatus)
            {
                item.Value.CheckSelf();
                item.Value.Reduce();
            }
        }
    }
}

