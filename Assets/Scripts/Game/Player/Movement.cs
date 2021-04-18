using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class Movement : LogicComponent
    {
        private List<Movement> _allMovement = new List<Movement>();

        private float _speedHorizontal;
        private float _speedVertical;
        private float _runSpeedMin;

        public bool canWalk = true;
        public bool canRun = true;
        public bool canJump = true;
        public bool isRight = true;
        public bool isPush = false;
        public bool isPull = false;

        public bool isWalk { get { return Mathf.Abs(speedHorizontal) > 0 && Mathf.Abs(speedHorizontal) < runSpeedMin && !isInAir; } }
        public bool isRun { get { return Mathf.Abs(speedHorizontal) >= runSpeedMin && !isInAir; } }
        public bool isInAir;



        //记得设置
        public float runSpeedMin
        {
            get { return _runSpeedMin; }
            set { _runSpeedMin = value; }
        }

        public float speedHorizontal
        {
            get
            { return _speedHorizontal; }
            set
            {
                if (!canWalk)
                {
                    _speedHorizontal = 0;
                }
                else if (!canRun)
                {
                    _speedHorizontal = Mathf.Min(value, runSpeedMin);
                }
                _speedHorizontal = value;
            }
        }

        public float speedVertical
        {
            get
            { return _speedVertical; }
            set
            {
                _speedVertical = value;
            }
        }

        public float speedVerticalFake
        {
            get
            {
                if (isInAir)
                    return _speedVertical;
                else
                    return 0;
            }
        }

        public float moveSpeedFake
        {
            get
            {
                if (isInAir)
                    return moveSpeed;
                else
                    return (Vector3.right * speedHorizontal).magnitude;
            }
        }

        //总速度的模
        public float moveSpeed { get { return moveVec.magnitude; } }
        //速度的向量
        public Vector3 moveVec { get { return Vector3.right * speedHorizontal + Vector3.up * speedVertical; } }

        public override void OnCreate()
        {
            base.OnCreate();
            _allMovement.Add(this);
        }

        public override void OnDestory()
        {
            base.OnDestory();
            _allMovement.Remove(this);
        }
    }
}

