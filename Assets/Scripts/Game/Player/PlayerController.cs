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
        private float speedVerticalMinOnGround = -3;

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
            //真正的角度,localRotation.eulerAngles.y永远为正数（即使在Inspector面板中可以为负数）,将该正数转化成inspector面板中的数。
            float big = 360 - transform.localRotation.eulerAngles.y;
            float small = transform.localRotation.eulerAngles.y;
            float angle = big < small ? -big : small;

            if (movement.isRight && angle < 90)
            {
                //向右转
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y + rotationDelta, transform.localRotation.z));
            }
            if (!movement.isRight && angle > -90)
            {
                //向左转
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y - rotationDelta, transform.localRotation.z));
            }
            if (canMove)
            {
                if (Mathf.Abs(movement.speedHorizontal) < horizontalSpeedMax)
                {
                    float horizontalMove = horizontal > 0 ? 1 : -1;
                    //水平速度
                    movement.speedHorizontal += horizontalMove*horizontalAcceleration * Time.fixedDeltaTime;
                }
                else
                {
                    //如果水平速度大于水平速度最大值且当前前进方向和速度方向一致，则将水平速度设置为最大值。
                    if (movement.speedHorizontal < 0&& !movement.isRight)
                    {
                        movement.speedHorizontal = -horizontalSpeedMax;
                    }
                    else if(movement.speedHorizontal > 0 && movement.isRight)
                    {
                        movement.speedHorizontal = horizontalSpeedMax;
                    }else
                        //当水平速度大于水平速度最大值且当前前进方向和速度方向不一致时，要将速度设置为0，否则就会出现按了D键还是在往左走的情况。
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
            else
            {
                //此举是为了保证玩家在下坡的时候isInAir始终为true，如果不让speedVertical减小的话，可能会出现如：
                //跳跃到一个高坡上speedVertical为 - 1，然后下坡的时候向下的速度不够导致characterController的碰不到地面而浮空。
                if (movement.speedVertical > speedVerticalMinOnGround)
                    movement.speedVertical -= 0.01f;
                else if (movement.speedVertical < speedVerticalMinOnGround)
                    movement.speedVertical += 0.01f;
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

