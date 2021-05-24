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
        private bool _shouldFallDown;

        private bool _canWalk = true;
        private bool _canRun = true;
        private bool _canJump = true;
        private bool _canPushPull = true;

        //输入的条件和参数
        private bool _readyForJump = false;
        private bool _readyForPush = false;
        private bool _readyForBreakPush = false;
        private bool _readyForMoveTowardsEndInSpeed = false;
        private MoveTowardsParameter _moveTowardsPara = null;
        private float _horizontalInput;
        //真正的属性
        private float _realJumpSpeed;
        private float _realHorizontalSpeedMax;
        private bool _changeHorizontalSpeedLock = false;
        private float _realGravity;
        private Vector3 _realGravityVec;

        public bool canMove { get { return _canWalk; } set { _canWalk = value; } }
        public bool canRun { get { return _canRun; } set { _canRun = value; } }
        public bool canJump { get { return _canJump; } set { _canJump = value; } }
        public bool canPushPull { get { return _canPushPull; } set { _canPushPull = value; } }
        public bool readyForJump { get { return _readyForJump; } set { _readyForJump = value; } }
        public bool readyForPush { get { return _readyForPush; } set { _readyForPush = value; } }
        public bool readyForBreakPush { get { return _readyForBreakPush; } set { _readyForBreakPush = value; } }
        public bool readyForMoveTowardsEndInSpeed { get { return _readyForMoveTowardsEndInSpeed; } set { _readyForMoveTowardsEndInSpeed = value; } }
        public MoveTowardsParameter moveTowardsPara { get { return _moveTowardsPara; } set { _moveTowardsPara = value; } }
        public float horizontalInput { get { return _horizontalInput; } set { _horizontalInput = value; } }

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
        private CharacterController _cc;
        private Rigidbody _body;
        private LogicContainer _container;
        private Movement _movement;
        private Animator _animator;
        private MovementAnimatorSetter _movementAnimSetter;
        [HideInInspector]
        public Vector3 center { get { return transform.position + _cc.center; } }

        private bool _isInAir;
        private bool _isRight = true;
        private float _speedHorizontal;
        private float _speedVertical;
        private bool _isPull;
        private bool _isPush;
        private Vector3 _moveVec;

        private void SetMovementInfo()
        {
            _movement.isInAir = _isInAir;
            _movement.isRight = _isRight;
            _movement.speedHorizontal = _speedHorizontal;
            _movement.speedVertical = _speedVertical;
            _movement.isPull = _isPull;
            _movement.isPush = _isPush;
            _movement.moveVec = _moveVec;
        }

        void Start()
        {
            _animator = GetComponent<Animator>();
            _container = GetComponent<LogicContainer>();
            _movement = _container.GetSingletonLogicCompoent<Movement>();
            if(_movement == null)
            {
                DebugHelper.LogError(gameObject.name+"没有movement");
                return;
            }
            _movementAnimSetter = _container.GetSingletonLogicCompoent<MovementAnimatorSetter>();
            if (_movement == null)
            {
                DebugHelper.LogError(gameObject.name + "没有MovementAnimatorSetter");
                return;
            }
            _movementAnimSetter.InitAnimatorInfo();
            _realJumpSpeed = jumpSpeed;
            _realHorizontalSpeedMax = horizontalSpeedMax;
            _realGravity = gravity;
            _realGravityVec = gravityVec;
            _container = GetComponent<LogicContainer>();
            _movement.runSpeedMin = horizontalRunSpeedMin;
            _cc = GetComponent<CharacterController>();
            _body = GetComponent<Rigidbody>();
            _body.useGravity = false;
            _body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            _body.sleepThreshold = 0;
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
            if (_cc.isGrounded)
            {
                _isInAir = false;
                if (_changeHorizontalSpeedLock == false)
                    ResetRealHorizontalSpeed(false);
            }
            else
            {
                _isInAir = true;
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

        private void HorizontalMove()
        {
            if (!CheckMove())
                return;
            DebugHelper.Log(_isInAir ? "isInAir" : "notInAir");
            bool isMove = false;
            if (_horizontalInput != 0)
            {
                isMove = true;
                _isRight = _horizontalInput > 0;
            }
            //真正的角度,localRotation.eulerAngles.y永远为正数（即使在Inspector面板中可以为负数）,将该正数转化成inspector面板中的数。
            float big = 360 - transform.localRotation.eulerAngles.y;
            float small = transform.localRotation.eulerAngles.y;
            float angle = big < small ? -big : small;

            AnimatorStateInfo info = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            if (_isRight && angle < 90 && !info.IsName("PushStart") && !info.IsName("PushMotion") && !info.IsName("Push Stop") && !info.IsName("PullStart") && !info.IsName("PullMotion"))
            {
                //向右转
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y + _rotationDelta, transform.localRotation.z));
                canPushPull = false;
            }
            else if (!_isRight && angle > -90 && !info.IsName("PushStart") && !info.IsName("PushMotion") && !info.IsName("Push Stop") && !info.IsName("PullStart") && !info.IsName("PullMotion"))
            {
                //向左转
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.eulerAngles.y - _rotationDelta, transform.localRotation.z));
                canPushPull = false;
            }
            else
                canPushPull = true;
            if (isMove)
            {
                if (!canRun)
                {
                    if (_changeHorizontalSpeedLock == false)
                        _realHorizontalSpeedMax = horizontalRunSpeedMin;
                }
                if (Mathf.Abs(_speedHorizontal) < _realHorizontalSpeedMax)
                {
                    float horizontalMove = _horizontalInput > 0 ? 1 : -1;
                    //水平速度
                    _speedHorizontal += horizontalMove * horizontalAcceleration * Time.fixedDeltaTime;
                }
                else
                {
                    //如果水平速度大于水平速度最大值且当前前进方向和速度方向一致，则将水平速度设置为最大值。
                    if (_speedHorizontal < 0 && !_isRight)
                    {
                        _speedHorizontal = -_realHorizontalSpeedMax;
                    }
                    else if (_speedHorizontal > 0 && _isRight)
                    {
                        _speedHorizontal = _realHorizontalSpeedMax;
                    }
                }
                if (_speedHorizontal < 0 && _isRight || _speedHorizontal > 0 && !_isRight)
                {
                    //当水平速度大于水平速度最大值且当前前进方向和速度方向不一致时，要将速度设置为0，否则就会出现按了D键还是在往左走的情况。
                    _speedHorizontal = 0;
                }
            }
            else
            {
                if(!_isInAir)
                    _speedHorizontal = 0;
            }
            
            //暂停拉/推动画
            if ((info.IsName("PushMotion") || info.IsName("PullMotion")) && _speedHorizontal == 0 && (_isPush || _isPull))
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
            return !_isInAir && canJump;
        }

        private void Jump()
        {
            if (!CheckJump())
                return;
            BreakPush();
            _speedVertical = _realJumpSpeed;
            _readyForJump = false;
        }

        private bool CheckFallDown()
        {
            return _isInAir || _shouldFallDown;
        }

        private void FallDown()
        {
            if (!CheckFallDown())
                return;

            //如果从高处跳下，则要将垂直速度置0
            if (_isInAir != _oldIsInAir && _speedVertical < 0)
                _speedVertical = 0;
            _speedHorizontal += _realGravityVec.x * _realGravity * Time.fixedDeltaTime;
            _speedVertical += _realGravityVec.y * _realGravity * Time.fixedDeltaTime;
        }

        private void ChangeOldIsInAir()
        {
            _oldIsInAir = _isInAir;
        }

        private void ChangeVerticalSpeedOnGround()
        {
            if (!_isInAir)
            {
                //此举是为了保证玩家在下坡的时候isInAir始终为true，如果不让speedVertical减小的话，可能会出现如：
                //跳跃到一个高坡上speedVertical为 - 1，然后下坡的时候向下的速度不够导致characterController的碰不到地面而浮空。
                //if (_speedVertical > _speedVerticalMinOnGround)
                //    _speedVertical -= 0.01f;
                //else if (_speedVertical < _speedVerticalMinOnGround)
                //    _speedVertical += 0.01f;
                _speedVertical = _speedVerticalMinOnGround;
            }
        }

        private bool CheckPush()
        {
            return (!_isInAir && canPushPull);
        }

        private void Push()
        {
            if (!CheckPush())
                return;
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.right * (_isRight ? 1 : -1), out hit, raycastDistance, raycastMask))
            {
                if (pushObj == null)
                {
                    GameObject obj = hit.collider.gameObject;
                    PushableObject push = obj.GetComponent<PushableObject>();
                    if (push != null)
                    {
                        push.Connect(_movement);
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
                    if (_isRight)
                    {
                        _isPush = true;
                        _isPull = false;
                    }
                    else
                    {
                        _isPush = false;
                        _isPull = true;
                    }
                }
                else if (pushObj.gameObject.transform.position.x < transform.position.x)
                {
                    if (_isRight)
                    {
                        _isPush = false;
                        _isPull = true;
                    }
                    else
                    {
                        _isPush = true;
                        _isPull = false;
                    }
                }
            }
            else
            {
                _isPush = false;
                _isPull = false;
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
            _isPush = false;
            _isPull = false;
            _readyForBreakPush = false;
        }

        private void CharacterControllerMove()
        {
            _moveVec = Vector3.right * _speedHorizontal + Vector3.up * _speedVertical;
            _cc.Move(_moveVec * Time.fixedDeltaTime);
        }

        private void ClearStatus()
        {
            _readyForJump = false;
            _readyForPush = false;
            _readyForBreakPush = false;
            _horizontalInput = 0;
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
            _cc.enabled = false;
            transform.DOMove(_moveTowardsPara.target - _cc.center, _moveTowardsPara.duration, false).onComplete = () =>
            {
                _cc.enabled = true;
                _speedHorizontal = _moveTowardsPara.endSpeedVec.x * _moveTowardsPara.endSpeed;
                _speedVertical = _moveTowardsPara.endSpeedVec.y * _moveTowardsPara.endSpeed;
                _moveTowardsPara = null;
            };
            _readyForMoveTowardsEndInSpeed = false;
        }

        private void FixedUpdate()
        {
            CheckOnGround();
            ChangeVerticalSpeedOnGround();
            if (readyForJump)
                Jump();
            if (readyForPush)
                Push();
            if (readyForBreakPush || CheckPushObjDistanceCanBreak())
                BreakPush();
            if (readyForMoveTowardsEndInSpeed)
                MoveTowardsEndInSpeed();
            HorizontalMove();
            FallDown();
            ChangeOldIsInAir();
            CharacterControllerMove();
            SetMovementInfo();
            _movementAnimSetter.SetAnimatorInfo(_movement);
            ClearStatus();
        }
    }
}

