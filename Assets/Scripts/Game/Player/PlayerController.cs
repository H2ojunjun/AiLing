using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    //所有的玩家物理效果的接口都在此
    [RequireComponent(typeof(CharacterController),typeof(Rigidbody))]
    public class PlayerController : MonoSingleton<PlayerController>
    {
        public Movement movement;
        [LabelText("跳跃初速度")]
        public float jumpSpeed = 6;
        [LabelText("水平加速度")]
        public float horizontalAcceleration = 10;
        [LabelText("水平速度最大值")]
        public float horizontalSpeedMax = 50;
        [HideInInspector]
        public CharacterController cc;
        public bool isInAir { get { return movement.isInAir; } set { movement.isInAir = value; } }
        public MovementAnimatorSetter movementAnimSetter;

        private bool oldIsInAir;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            movement = new Movement(this);
            cc = GetComponent<CharacterController>();
            movementAnimSetter = new MovementAnimatorSetter(GetComponent<Animator>());
            movementAnimSetter.InitMovementAnimatorInfo();
            Rigidbody body = GetComponent<Rigidbody>();
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezePositionZ|RigidbodyConstraints.FreezeRotation;
        }

        private void Move()
        {
            CheckOnGround();
            Debug.Log(isInAir ? "isInAir" : "notInAir");
            float horizontal = Input.GetAxis("Horizontal");
            if (horizontal != 0)
            {
                if (Mathf.Abs(movement.speedHorizontal) < horizontalSpeedMax)
                    movement.speedHorizontal += horizontal * horizontalAcceleration * Time.deltaTime;
                else
                {
                    if (movement.speedHorizontal < 0)
                        movement.speedHorizontal = -horizontalSpeedMax;
                    else
                        movement.speedHorizontal = horizontalSpeedMax;
                }
            }
            else
            {
                movement.speedHorizontal = 0;
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space))
            {
                if (!isInAir)
                {
                    movement.Jump();
                }
            }
            if (isInAir)
            {
                //如果从高处跳下，则要将垂直速度置0
                if (isInAir != oldIsInAir && movement.speedVertical < 0)
                    movement.speedVertical = 0;
                movement.FallingDown(Time.deltaTime);
            }
            cc.Move(movement.moveVec * Time.deltaTime);
            oldIsInAir = isInAir;
        }

        private void CheckOnGround()
        {
            if (cc.isGrounded)
                isInAir = false;
            else
                isInAir = true;
        }

        private void Update()
        {
            Move();
            movementAnimSetter.SetMovementAnimatorInfo(movement);
        }
    }
}

