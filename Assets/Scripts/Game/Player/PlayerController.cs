using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    //所有的玩家物理效果的接口都在此
    [RequireComponent(typeof(CharacterController), typeof(Rigidbody))]
    public class PlayerController : MonoSingleton<PlayerController>
    {
        private bool _oldIsInAir;
        private float rotationDelta;

        public Movement movement;
        [LabelText("跳跃初速度")]
        public float jumpSpeed = 6;
        [LabelText("水平加速度")]
        public float horizontalAcceleration = 10;
        [LabelText("水平速度最大值")]
        public float horizontalSpeedMax = 50;
        [LabelText("转向时间")]
        public float roationTime = 1;
        [HideInInspector]
        public CharacterController cc;
        public bool isInAir { get { return movement.isInAir; } set { movement.isInAir = value; } }
        public MovementAnimatorSetter movementAnimSetter;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            movement = new Movement(this);
            cc = GetComponent<CharacterController>();
            movementAnimSetter = new MovementAnimatorSetter(GetComponent<Animator>());
            movementAnimSetter.InitMovementAnimatorInfo();
            Rigidbody body = GetComponent<Rigidbody>();
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            rotationDelta = 90 / (roationTime / Time.fixedDeltaTime);
        }

        private void Move()
        {
            CheckOnGround();
            Debug.Log(isInAir ? "isInAir" : "notInAir");
            float horizontal = Input.GetAxis("Horizontal");
            bool canMove = false;
            if (horizontal != 0)
            {
                canMove = true;
                movement.isRight = horizontal > 0;
            }
            //真正的角度
            float big = 360 - transform.localRotation.eulerAngles.y;
            float small = transform.localRotation.eulerAngles.y;
            float angle = big < small ? -big : small;
            if (movement.isRight && angle < 90)
            {
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y + rotationDelta, transform.localRotation.z));
            }
            if (!movement.isRight && angle > -90)
            {
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y - rotationDelta, transform.localRotation.z));
            }
            if (canMove)
            {
                if (Mathf.Abs(movement.speedHorizontal) < horizontalSpeedMax)
                {
                    float horizontalMove = horizontal > 0 ? 1 : -1;
                    movement.speedHorizontal += horizontalMove*horizontalAcceleration * Time.fixedDeltaTime;
                }
                else
                {
                    if (movement.speedHorizontal < 0&& !movement.isRight)
                    {
                        movement.speedHorizontal = -horizontalSpeedMax;
                    }
                    else if(movement.speedHorizontal > 0 && movement.isRight)
                    {
                        movement.speedHorizontal = horizontalSpeedMax;
                    }else
                        movement.speedHorizontal = 0;
                }
            }
            else
            {
                movement.speedHorizontal = 0;
            }
            if (Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Space))
            {
                if (!isInAir)
                {
                    movement.Jump();
                }
            }
            if (isInAir)
            {
                //如果从高处跳下，则要将垂直速度置0
                if (isInAir != _oldIsInAir && movement.speedVertical < 0)
                    movement.speedVertical = 0;
                movement.FallingDown(Time.fixedDeltaTime);
            }
            cc.Move(movement.moveVec * Time.fixedDeltaTime);
            _oldIsInAir = isInAir;
        }

        private void CheckOnGround()
        {
            if (cc.isGrounded)
                isInAir = false;
            else
                isInAir = true;
        }

        //private void Update()
        //{
        //    Move();
        //    movementAnimSetter.SetMovementAnimatorInfo(movement);
        //}

        private void FixedUpdate()
        {
            Move();
            movementAnimSetter.SetMovementAnimatorInfo(movement);
        }
    }
}

