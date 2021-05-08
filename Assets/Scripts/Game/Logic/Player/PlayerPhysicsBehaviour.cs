using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace AiLing
{
    public class MoveTowardsParameter
    {
        public Vector3 target;
        public Vector3 endSpeedVec;
        public float endSpeed;
        public float duration;

        public MoveTowardsParameter(Vector3 target, Vector3 endSpeedVec, float endSpeed, float duration)
        {
            this.target = target;
            this.endSpeedVec = endSpeedVec;
            this.endSpeed = endSpeed;
            this.duration = duration;
        }
    }

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
        private bool _shouldFallDown;

        private bool _canWalk = true;
        private bool _canRun = true;
        private bool _canJump = true;

        //突发事件触发条件和参数
        private bool _readyForJump = false;
        private bool _readyForPush = false;
        private bool _readyForBreakPush = false;
        private bool _readyForMoveTowardsEndInSpeed = false;
        private MoveTowardsParameter _moveTowardsPara = null;
        //真正的属性
        private float _realJumpSpeed;
        private float _realHorizontalSpeedMax;
        private bool _changeHorizontalSpeedLock = false;
        private float _realGravity;
        private Vector3 _realGravityVec;

        [HideInInspector]
        public bool canMove { get { return _canWalk; } set { _canWalk = value; } }
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
        public bool readyForMoveTowardsEndInSpeed { get { return _readyForMoveTowardsEndInSpeed; } set { _readyForMoveTowardsEndInSpeed = value; } }
        [HideInInspector]
        public MoveTowardsParameter moveTowardsPara { get { return _moveTowardsPara; } set { _moveTowardsPara = value; } }

        [LabelText("跳跃初速度")]
        [OnValueChanged("ResetRealJumpSpeed")]
        public float jumpSpeed = 7;
        [LabelText("重力加速度")]
        [OnValueChanged("ResetRealGravity")]
        public float gravity = 10;
        [LabelText("重力方向")]
        [OnValueChanged("ReSetRealGravityVec")]
        public Vector3 gravityVec = Vector3.down;
        [LabelText("重力检测距离")]
        public float gravityDetectionDistance = 0.95f;
        [LabelText("水平加速度")]
        public float horizontalAcceleration = 3;
        [LabelText("水平速度最大值")]
        [OnValueChanged("ResetRealHorizontalSpeedWithoutLock")]
        public float horizontalSpeedMax = 6;
        [LabelText("拉速度最大值")]
        public float pullHorizontalSpeedMax = 4;
        [LabelText("空中水平速度最大值")]
        public float horizontalSpeedMaxInAir = 2;
        [LabelText("最小奔跑速度")]
        public float horizontalRunSpeedMin = 3;
        [LabelText("转向时间")]
        public float roationTime = 0.2f;
        [LabelText("射线检测距离")]
        public float raycastDistance = 0.5f;
        [LabelText("射线检测layer")]
        public LayerMask raycastMask;
        [LabelText("重力检测layer")]
        public LayerMask gravityMask;
        [LabelText("切断push距离")]
        public float breakPushDis = 5;
        [ReadOnly]
        [LabelText("被推物体")]
        public PushableObject pushObj;
        [HideInInspector]
        public CharacterController cc;
        [HideInInspector]
        public Rigidbody body;
        [HideInInspector]
        public Vector3 center { get { return transform.position + cc.center; } }

        void Start()
        {
            _realJumpSpeed = jumpSpeed;
            _realHorizontalSpeedMax = horizontalSpeedMax;
            _realGravity = gravity;
            _realGravityVec = gravityVec;
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

        #region set,reset
        public void SetRealHorizontalSpeed(float speed, bool changeLock = true)
        {
            _realHorizontalSpeedMax = speed;
            if (changeLock)
                _changeHorizontalSpeedLock = true;
        }

        public void ResetRealHorizontalSpeed(bool changeLock = true)
        {
            _realHorizontalSpeedMax = horizontalSpeedMax;
            if (changeLock)
                _changeHorizontalSpeedLock = false;
        }

        private void ResetRealHorizontalSpeedWithoutLock()
        {
            ResetRealHorizontalSpeed(false);
        }

        public void SetRealJumpSpeed(float speed)
        {
            _realJumpSpeed = speed;
        }

        public void ResetRealJumpSpeed()
        {
            _realJumpSpeed = jumpSpeed;
        }

        public void SetRealGravity(float g)
        {
            _realGravity = g;
        }

        public void ResetRealGravity()
        {
            _realGravity = gravity;
        }

        public void SetRealGravityVec(Vector3 vec)
        {
            _realGravityVec = vec;
        }

        public void ReSetRealGravityVec()
        {
            _realGravityVec = gravityVec;
        }
        #endregion

        private void CheckOnGround()
        {
            if (cc.isGrounded)
            {
                movement.isInAir = false;
                if (_changeHorizontalSpeedLock == false)
                    ResetRealHorizontalSpeed(false);
            }
            else
            {
                movement.isInAir = true;
                if (_changeHorizontalSpeedLock == false)
                    SetRealHorizontalSpeed(horizontalSpeedMaxInAir, false);
            }
        }

        private bool CheckMove()
        {
            if (!canMove)
            {
                return false;
            }
            return true;
        }

        private void Move()
        {
            if (!CheckMove())
                return;
            DebugHelper.Log(movement.isInAir ? "isInAir" : "notInAir");
            float horizontal = InputManager.Instance.GetHorizontal();
            bool isMove = false;
            if (horizontal != 0)
            {
                isMove = true;
                movement.isRight = horizontal > 0;
            }
            //真正的角度,localRotation.eulerAngles.y永远为正数（即使在Inspector面板中可以为负数）,将该正数转化成inspector面板中的数。
            float big = 360 - transform.localRotation.eulerAngles.y;
            float small = transform.localRotation.eulerAngles.y;
            float angle = big < small ? -big : small;

            AnimatorStateInfo info = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            if (movement.isRight && angle < 90 && !info.IsName("PushStart") && !info.IsName("PushMotion") && !info.IsName("Push Stop") && !info.IsName("PullStart") && !info.IsName("PullMotion"))
            {
                //向右转
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y + _rotationDelta, transform.localRotation.z));
            }
            if (!movement.isRight && angle > -90 && !info.IsName("PushStart") && !info.IsName("PushMotion") && !info.IsName("Push Stop") && !info.IsName("PullStart") && !info.IsName("PullMotion"))
            {
                //向左转
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y - _rotationDelta, transform.localRotation.z));
            }
            if (isMove)
            {
                if (!canRun)
                {
                    if (_changeHorizontalSpeedLock == false)
                        _realHorizontalSpeedMax = horizontalRunSpeedMin;
                }
                if (Mathf.Abs(movement.speedHorizontal) < _realHorizontalSpeedMax)
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
                        movement.speedHorizontal = -_realHorizontalSpeedMax;
                    }
                    else if (movement.speedHorizontal > 0 && movement.isRight)
                    {
                        movement.speedHorizontal = _realHorizontalSpeedMax;
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
                if(!movement.isInAir)
                    movement.speedHorizontal = 0;
            }
            
            //暂停拉/推动画
            if ((info.IsName("PushMotion") || info.IsName("PullMotion")) && movement.speedHorizontal == 0 && (movement.isPush || movement.isPull))
            {
                GetComponent<Animator>().speed = 0;
            }
            else
            {
                GetComponent<Animator>().speed = 1;
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
            movement.speedVertical = _realJumpSpeed;
            _readyForJump = false;
        }

        private bool CheckFallDown()
        {
            return movement.isInAir || _shouldFallDown;
        }

        private void FallDown()
        {
            if (!CheckFallDown())
                return;

            //如果从高处跳下，则要将垂直速度置0
            if (movement.isInAir != _oldIsInAir && movement.speedVertical < 0)
                movement.speedVertical = 0;
            movement.speedHorizontal += _realGravityVec.x * _realGravity * Time.fixedDeltaTime;
            movement.speedVertical += _realGravityVec.y * _realGravity * Time.fixedDeltaTime;
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
                        if (_changeHorizontalSpeedLock == false)
                        {
                            _realHorizontalSpeedMax = pullHorizontalSpeedMax;
                            _changeHorizontalSpeedLock = true;
                        }
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
            _realHorizontalSpeedMax = horizontalSpeedMax;
            _changeHorizontalSpeedLock = false;
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
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            CheckGravity(hit);
        }

        private void CheckGravity(ControllerColliderHit hit)
        {
            GameObject obj = hit.gameObject;
            if (obj.layer == 0 || obj.layer == 6)
            {
                if (Vector3.Dot(hit.normal.normalized, _realGravityVec) > 0)
                {
                    _shouldFallDown = true;
                    return;
                }
            }
            _shouldFallDown = false;
        }

        private bool CheckMoveTowardsEndInSpeed()
        {
            return true;
        }

        //移动到某个点且抵达时刚好速度为某个值
        private void MoveTowardsEndInSpeed()
        {
            if (_moveTowardsPara == null)
            {
                DebugHelper.LogError(transform.name + "没有moveTowardsPara!");
                return;
            }
            if (!CheckMoveTowardsEndInSpeed())
                return;
            cc.enabled = false;
            transform.DOMove(_moveTowardsPara.target - cc.center, _moveTowardsPara.duration, false).onComplete = () =>
            {
                cc.enabled = true;
                movement.speedHorizontal = _moveTowardsPara.endSpeedVec.x * _moveTowardsPara.endSpeed;
                movement.speedVertical = _moveTowardsPara.endSpeedVec.y * _moveTowardsPara.endSpeed;
                _moveTowardsPara = null;
            };
            _readyForMoveTowardsEndInSpeed = false;
        }

        private void FixedUpdate()
        {
            CheckOnGround();
            if (readyForJump)
                Jump();
            if (readyForPush)
                Push();
            if (readyForBreakPush || CheckPushObjDistanceCanBreak())
                BreakPush();
            if (readyForMoveTowardsEndInSpeed)
                MoveTowardsEndInSpeed();
            Move();
            FallDown();
            ChangeVerticalSpeedOnGround();
            CharacterControllerMove();
            ClearStatus();
        }
    }
}

