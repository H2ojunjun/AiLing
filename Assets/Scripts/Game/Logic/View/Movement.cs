using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    //玩家移动信息储存地
    public class Movement : LogicComponent
    {
        private List<Movement> _allMovement = new List<Movement>();

        private float _speedHorizontal;
        private float _speedVertical;
        private float _runSpeedMin;
        private Vector3 _moveVec;

        private bool _isRight = true;
        private bool _isPush = false;
        private bool _isPull = false;
        private bool _isInAir = false;
        private bool _isPress = false;
        private bool _isInteract = false;

        public bool isRight { get { return _isRight; }set { _isRight = value; } }
        public bool isPush { get { return _isPush; } set { _isPush = value; } }
        public bool isPull { get { return _isPull; } set { _isPull = value; } }
        public bool isInAir { get { return _isInAir; } set { _isInAir = value; } }
        public bool isPress { get { return _isPress; } set { _isPress = value; } }
        public bool isInteract { get { return _isInteract; } set { _isInteract = value; } }
        public bool isWalk { get { return Mathf.Abs(speedHorizontal) > 0 && Mathf.Abs(speedHorizontal) < runSpeedMin && !isInAir; } }
        public bool isRun { get { return Mathf.Abs(speedHorizontal) >= runSpeedMin && !isInAir; } }

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
        public Vector3 moveVec
        {
            get
            {
                return _moveVec;
            }
            set
            {
                _moveVec = value;
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _allMovement.Add(this);
        }

        public override void Init(GameObject obj)
        {
        }

        public override void OnDestory()
        {
            base.OnDestory();
            _allMovement.Remove(this);
        }
    }
}

