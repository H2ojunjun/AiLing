using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    //所有的玩家物理效果的接口都在此
    public class PlayerController : MonoSingleton<PlayerController>
    {
        //private Rigidbody _body;
        //private CapsuleCollider _col;
        private Vector3 _bottom, _top;
        private float overLapCapsuleOffset;
        private float _radius;
        private float _height;
        //private float _groundCheckLock=0f;
        private int jumpTimer;

        [HideInInspector]
        public TimerManager timerManager;
        [HideInInspector]
        public LayerMask igonreLayer;
        public Movement movement;
        public float jumpSpeed = 6;
        public float horizontalSpeed = 10;
        [HideInInspector]
        public CharacterController cc;
        public bool isInAir { get { return movement.isInAir; } set { movement.isInAir = value; } }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            timerManager = TimerManager.Instance;
            movement = new Movement(this);
            //_body = GetComponent<Rigidbody>();
            cc = GetComponent<CharacterController>();
            //_col = GetComponent<CapsuleCollider>();
            _radius = cc.radius * 0.9f;
            _height = cc.height;
            overLapCapsuleOffset = -(cc.height / 2 + 0.1f);
            igonreLayer = (1 << LayerMask.NameToLayer("Ground"));

        }

        //private void Move()
        //{
        //    CheckOnGround();
        //    Debug.Log(movement.isInAir ? "isInAir" : "notInAir");
        //    float horizontal = Input.GetAxis("Horizontal");
        //    movement.speedHorizontal = horizontal * horizontalSpeed;
        //    if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space))
        //    {
        //        if (!isInAir)
        //        {
        //            StartGroundLock();
        //            movement.Jump(_body);
        //            //_body.AddForce(new Vector3(0, 10, 0), ForceMode.VelocityChange);
        //            //Debug.Log(isInAir ? "inAir" : "notInAir");
        //        }
        //    }
        //    if (isInAir)
        //        movement.FallingDown(_body, Time.fixedDeltaTime);
        //    //Debug.Log(_body.velocity);
        //    //_body.velocity = new Vector3(movement.moveVec.x, movement.speedVertical, movement.moveVec.z);
        //    //_body.velocity = movement.moveVec;
        //    _body.MovePosition(this.transform.position + movement.moveVec * Time.fixedDeltaTime);
        //    //Debug.Log("movevec:" + movement.moveVec);
        //    //if (isInAir)
        //    //    Debug.Log("velocity:" + _body.velocity);
        //    //_body.MovePosition(this.transform.position + movement.moveVec * Time.fixedDeltaTime);
        //}


        private void Move()
        {
            CheckOnGround();
            Debug.Log(isInAir ? "isInAir" : "notInAir");
            float horizontal = Input.GetAxis("Horizontal");
            movement.speedHorizontal = horizontal * horizontalSpeed;
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space))
            {
                if (!isInAir)
                {
                    if (jumpTimer == 0)
                        jumpTimer = timerManager.AddTimer(0.2f, 0f, -1, null, null, () => { timerManager.Pause(jumpTimer); });
                    else
                        if (!timerManager.IsLeft(jumpTimer))
                        timerManager.Continue(jumpTimer);
                    //StartGroundLock();
                    movement.Jump();
                    //_body.AddForce(new Vector3(0, 10, 0), ForceMode.VelocityChange);
                    //Debug.Log(isInAir ? "inAir" : "notInAir");
                }
            }
            if (isInAir)
                movement.FallingDown(Time.deltaTime);
            cc.Move(movement.moveVec * Time.deltaTime);
        }
        private void CheckOnGround()
        {
            Timer timer = timerManager.GetTimer(jumpTimer);
            if (timer != null && timer.leftTime > 0)
                return;
            _bottom = transform.position + cc.center + transform.up * overLapCapsuleOffset + transform.up * _radius;
            _top = transform.position + cc.center + transform.up * _height / 2 - transform.up * _radius;

            Collider[] colliders = Physics.OverlapCapsule(_bottom, _top, _radius, igonreLayer);
#if UNITY_EDITOR
            Debug.DrawLine(_bottom, _top, Color.white);
#endif
            if (colliders.Length != 0)
            {
                //foreach (var col in colliders)
                //{
                //    if (!col.isTrigger)
                //    {
                        movement.speedVertical = 0;
                        isInAir = false;
                //    }
                //}
            }
            else
            {
                isInAir = true;
            }
        }

        //public void StartGroundLock()
        //{
        //    _groundCheckLock = 0.2f;
        //}

        //public void UpdateLock()
        //{
        //    _groundCheckLock -= Time.deltaTime;
        //    if (_groundCheckLock <= 0)
        //        _groundCheckLock = 0;
        //}

        private void Update()
        {
            Move();
            //UpdateLock();
        }

        //private void FixedUpdate()
        //{
        //    //Move();
        //    //UpdateLock();
        //}
    }
}

