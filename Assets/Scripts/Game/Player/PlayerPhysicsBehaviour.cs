using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    [RequireComponent(typeof(CharacterController), typeof(Rigidbody), typeof(LogicContainer))]
    public class PlayerPhysicsBehaviour : MonoBehaviour
    {
        private bool _oldIsInAir;
        //一次fixedUpdate的旋转偏移
        private float _rotationDelta;
        //地面上垂直速度的最小值
        private float _speedVerticalMinOnGround = -3;
        private LogicContainer _container;
        private Movement movement;

        private bool _canWalk = true;
        private bool _canRun = true;
        private bool _canJump = true;

        private bool _readyForJump = false;
        private bool _readyForPush = false;
        private bool _readyForBreakPush = false;
        //临时的跳跃速度
        private Vector3 _temporyJumpVec = Vector3.zero;

        [HideInInspector]
        public bool canWalk { get { return _canWalk; } set { _canWalk = value; } }
        [HideInInspector]
        public bool canRun { get { return _canRun; } set { _canRun = value; } }
        [HideInInspector]
        public bool canJump { get { return _canJump; } set { _canJump = value; } }
        [HideInInspector]
        public bool readyForJump { get { return _readyForJump; } set { _readyForJump = value; } }
        [HideInInspector]
        public bool readyForPush { get { return _readyForPush; } set { _readyForPush = value; } }
        [HideInInspector]
        public bool readyForBreakPush { get { return _readyForBreakPush; } set { _readyForBreakPush = value; } }
        [HideInInspector]
        public Vector3 temporyJumpVec { get { return _temporyJumpVec; } set { _temporyJumpVec = value; } }

        [LabelText("跳跃初速度")]
        public float jumpSpeed = 7;
        [LabelText("重力加速度")]
        public float gravity = 10;
        [LabelText("水平加速度")]
        public float horizontalAcceleration = 3;
        [LabelText("水平速度最大值")]
        public float horizontalSpeedMax = 6;
        [LabelText("最小奔跑速度")]
        public float horizontalRunSpeedMin = 3;
        [LabelText("转向时间")]
        public float roationTime = 0.2f;
        [LabelText("射线检测距离")]
        public float raycastDistance = 0.5f;
        [LabelText("射线检测layer")]
        public LayerMask raycastMask;
        [LabelText("切断push距离")]
        public float breakPushDis = 5;
        [ReadOnly]
        [LabelText("被推物体")]
        public PushableObject pushObj;
        [HideInInspector]
        public CharacterController cc;
        [HideInInspector]
        public Rigidbody body;

        void Start()
        {
            _container = GetComponent<LogicContainer>();
            movement = _container.AddSingletonLogicComponent<Movement>();
            movement.runSpeedMin = horizontalRunSpeedMin;
            cc = GetComponent<CharacterController>();
            body = GetComponent<Rigidbody>();
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            body.sleepThreshold = 0;
            _rotationDelta = 90 / (roationTime / Time.fixedDeltaTime);
        }

        private void CheckOnGround()
        {
            if (cc.isGrounded)
                movement.isInAir = false;
            else
                movement.isInAir = true;
        }

        private bool CheckMove()
        {
            if (!canWalk)
            {
                movement.speedHorizontal = 0;
                return false;
            }
            return true;
        }

        private void Move()
        {
            if (!CheckMove())
                return;
            CheckOnGround();
            Debug.Log(movement.isInAir ? "isInAir" : "notInAir");
            float horizontal = InputManager.Instance.GetHorizontal();
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
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y + _rotationDelta, transform.localRotation.z));
            }
            if (!movement.isRight && angle > -90)
            {
                //向左转
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y - _rotationDelta, transform.localRotation.z));
            }
            if (canMove)
            {
                float realHorizontalSpeedMax = horizontalSpeedMax;
                if (!canRun)
                    realHorizontalSpeedMax = horizontalRunSpeedMin;
                if (Mathf.Abs(movement.speedHorizontal) < realHorizontalSpeedMax)
                {
                    float horizontalMove = horizontal > 0 ? 1 : -1;
                    //水平速度
                    movement.speedHorizontal += horizontalMove * horizontalAcceleration * Time.fixedDeltaTime;
                }
                else
                {
                    //如果水平速度大于水平速度最大值且当前前进方向和速度方向一致，则将水平速度设置为最大值。
                    if (movement.speedHorizontal < 0 && !movement.isRight)
                    {
                        movement.speedHorizontal = -realHorizontalSpeedMax;
                    }
                    else if (movement.speedHorizontal > 0 && movement.isRight)
                    {
                        movement.speedHorizontal = realHorizontalSpeedMax;
                    }
                }
                if (movement.speedHorizontal < 0 && movement.isRight || movement.speedHorizontal > 0 && !movement.isRight)
                {
                    //当水平速度大于水平速度最大值且当前前进方向和速度方向不一致时，要将速度设置为0，否则就会出现按了D键还是在往左走的情况。
                    movement.speedHorizontal = 0;
                }
            }
            else
            {
                movement.speedHorizontal = 0;
            }
        }

        private bool CheckJump()
        {
            return !movement.isInAir && canJump;
        }

        private void Jump()
        {
            if (!CheckJump())
                return;
            BreakPush();
            if (_temporyJumpVec != Vector3.zero)
            {
                movement.speedVertical = _temporyJumpVec.x;
                movement.speedHorizontal = _temporyJumpVec.y;
            }
            else
            {
                movement.speedVertical = jumpSpeed;
            }
            _temporyJumpVec = Vector3.zero;
            _readyForJump = false;
        }

        private bool CheckFallDown()
        {
            return movement.isInAir;
        }

        private void FallDown()
        {
            if (!CheckFallDown())
                return;
            //如果从高处跳下，则要将垂直速度置0
            if (movement.isInAir != _oldIsInAir && movement.speedVertical < 0)
                movement.speedVertical = 0;
            movement.speedVertical = movement.speedVertical - gravity * Time.fixedDeltaTime;
            _oldIsInAir = movement.isInAir;
        }

        private void ChangeVerticalSpeedOnGround()
        {
            if (!movement.isInAir)
            {
                //此举是为了保证玩家在下坡的时候isInAir始终为true，如果不让speedVertical减小的话，可能会出现如：
                //跳跃到一个高坡上speedVertical为 - 1，然后下坡的时候向下的速度不够导致characterController的碰不到地面而浮空。
                if (movement.speedVertical > _speedVerticalMinOnGround)
                    movement.speedVertical -= 0.01f;
                else if (movement.speedVertical < _speedVerticalMinOnGround)
                    movement.speedVertical += 0.01f;
            }
        }

        private bool CheckPush()
        {
            if (movement.isInAir)
                return false;
            return true;
        }

        private void Push()
        {
            if (!CheckPush())
                return;
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.right * (movement.isRight ? 1 : -1), out hit, raycastDistance, raycastMask))
            {
                if (pushObj == null)
                {
                    GameObject obj = hit.collider.gameObject;
                    PushableObject push = obj.GetComponent<PushableObject>();
                    if (push != null)
                    {
                        push.Connect(movement);
                        pushObj = push;
                    }
                }
            }
            if (pushObj != null)
            {
                if (pushObj.gameObject.transform.position.x > transform.position.x)
                {
                    if (movement.isRight)
                    {
                        movement.isPush = true;
                        movement.isPull = false;
                    }
                    else
                    {
                        movement.isPush = false;
                        movement.isPull = true;
                    }
                }
                else if (pushObj.gameObject.transform.position.x < transform.position.x)
                {
                    if (movement.isRight)
                    {
                        movement.isPush = false;
                        movement.isPull = true;
                    }
                    else
                    {
                        movement.isPush = true;
                        movement.isPull = false;
                    }
                }
            }
            else
            {
                movement.isPush = false;
                movement.isPull = false;
            }
            _readyForPush = false;
        }

        private bool CheckBreakPush()
        {
            return pushObj != null;
        }

        private bool CheckPushObjDistanceCanBreak()
        {
            if (!CheckBreakPush())
                return false;
            return Vector3.Distance(transform.position, pushObj.transform.position) > breakPushDis;
        }

        private void BreakPush()
        {
            if (!CheckBreakPush())
                return;
            pushObj.Break();
            pushObj = null;
            movement.isPush = false;
            movement.isPull = false;
            _readyForBreakPush = false;
        }

        private void CharacterControllerMove()
        {
            cc.Move(movement.moveVec * Time.fixedDeltaTime);
        }

        private void ClearStatus()
        {
            _readyForJump = false;
            _readyForPush = false;
            _readyForBreakPush = false;
            _temporyJumpVec = Vector3.zero;
        }

        private void FixedUpdate()
        {
            Move();
            if (readyForJump)
                Jump();
            if (readyForPush)
                Push();
            if (readyForBreakPush || CheckPushObjDistanceCanBreak())
                BreakPush();
            FallDown();
            ChangeVerticalSpeedOnGround();
            CharacterControllerMove();
            ClearStatus();
        }
    }
}

