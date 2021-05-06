using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
    //所有的玩家物理效果的接口都在此
    [RequireComponent(typeof(CharacterController), typeof(Rigidbody), typeof(LogicContainer))]
    public class PlayerController : MonoSingleton<PlayerController>
    {
        private bool _oldIsInAir;
        //一次fixedUpdate的旋转偏移
        private float _rotationDelta;
        //地面上垂直速度的最小值
        private float _speedVerticalMinOnGround = -3;
        private int climbableMask = 11;
        //竖直和水平控制攀爬的分界余弦值,接触平面的法向量和vector2.right的夹角的余弦值的绝对值小于改值则水平控制攀爬，否之亦然
        private float horizontalVerticalDivide = Mathf.Cos(Mathf.PI / 4);
        //攀爬平面的法向量
        private Vector2 panelNormal = Vector2.zero;
        //用于检测攀爬的碰撞器
        private Collider climbCollider;
        //碰撞器路径
        private const string climbColliderPath = "colliders/climb";
        //取消攀爬碰撞检测Timer
        private int cancelClimbCollisionTimer;
        //攀爬上的动画clip
        private AnimationClip climbUpAnimationClip;

        [LabelText("跳跃初速度")]
        public float jumpSpeed = 6;
        [LabelText("重力加速度")]
        public float gravity = 10;
        [LabelText("水平加速度")]
        public float horizontalAcceleration = 10;
        [LabelText("水平速度最大值")]
        public float horizontalSpeedMax = 50;
        [LabelText("攀爬速度")]
        public float climbSpeed = 50;
        [LabelText("转向时间")]
        public float roationTime = 1;
        [LabelText("射线检测距离")]
        public float raycastDistance;
        [LabelText("射线检测layer")]
        public LayerMask raycastMask;
        [LabelText("切断push距离")]
        public float breakPushDis=5;
        [LabelText("右手")]
        [ReadOnly]
        public Transform rightHandTransform;

        [ReadOnly]
        [LabelText("被推物体")]
        public PushableObject pushObj;
        [HideInInspector]
        public CharacterController cc;
        [HideInInspector]
        public Rigidbody body;
        [HideInInspector]
        public Vector3 climbNormal = Vector3.zero;
        [HideInInspector]
        public LogicContainer container;
        [HideInInspector]
        public Animator animator;
        public Movement movement;
        public PlayerLife player;

        public bool isInAir { get { return movement.isInAir; } set { movement.isInAir = value; } }
        public MovementAnimatorSetter movementAnimSetter;
        public PlayerLifeAnimatorSetter playerLifeAnimatorSetter;

        void Start()
        {
            container = GetComponent<LogicContainer>();
            movement = container.AddSingletonLogicComponent<Movement>();
            movement.runSpeedMin = horizontalSpeedMax / 2;
            player = container.AddSingletonLogicComponent<PlayerLife>();
            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            AddAnimationEvent(animator, "Sprint To Wall Climb", "ClimbUpFinish", -1);
            movementAnimSetter = new MovementAnimatorSetter(animator);
            playerLifeAnimatorSetter = new PlayerLifeAnimatorSetter(animator);
            movementAnimSetter.InitAnimatorInfo();
            playerLifeAnimatorSetter.InitAnimatorInfo();
            body = GetComponent<Rigidbody>();
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            body.sleepThreshold = 0;
            climbCollider = transform.Find(climbColliderPath).GetComponent<Collider>();
            _rotationDelta = 90 / (roationTime / Time.fixedDeltaTime);
            enabled = false;
        }

        private void Move()
        {
            CheckOnGround();
            Debug.Log(isInAir ? "isInAir" : "notInAir");
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
                if (Mathf.Abs(movement.speedHorizontal) < horizontalSpeedMax)
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
                        movement.speedHorizontal = -horizontalSpeedMax;
                    }
                    else if (movement.speedHorizontal > 0 && movement.isRight)
                    {
                        movement.speedHorizontal = horizontalSpeedMax;
                    }
                    else
                        //当水平速度大于水平速度最大值且当前前进方向和速度方向不一致时，要将速度设置为0，否则就会出现按了D键还是在往左走的情况。
                        movement.speedHorizontal = 0;
                }
            }
            else
            {
                movement.speedHorizontal = 0;
            }
            if (InputManager.Instance.GetKeyDown(KeyCode.JoystickButton0) || InputManager.Instance.GetKeyDown(KeyCode.Space))
            {
                if (!isInAir)
                {
                    if (pushObj != null)
                    {
                        pushObj.Break();
                        pushObj = null;
                    }
                    Jump();
                }
            }
            if (isInAir)
            {
                //如果从高处跳下，则要将垂直速度置0
                if (isInAir != _oldIsInAir && movement.speedVertical < 0)
                    movement.speedVertical = 0;
                FallDown(Time.deltaTime);
            }
            else
            {
                //此举是为了保证玩家在下坡的时候isInAir始终为true，如果不让speedVertical减小的话，可能会出现如：
                //跳跃到一个高坡上speedVertical为 - 1，然后下坡的时候向下的速度不够导致characterController的碰不到地面而浮空。
                if (movement.speedVertical > _speedVerticalMinOnGround)
                    movement.speedVertical -= 0.01f;
                else if (movement.speedVertical < _speedVerticalMinOnGround)
                    movement.speedVertical += 0.01f;
            }
            cc.Move(movement.moveVec * Time.fixedDeltaTime);
            _oldIsInAir = isInAir;
        }

        private void Push()
        {
            if ((InputManager.Instance.GetKeyPress(KeyCode.Mouse0) || InputManager.Instance.GetKeyPress(KeyCode.JoystickButton1)) && !isInAir)
            {
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
            }
            if (pushObj != null)
            {
                if (InputManager.Instance.GetKeyUp(KeyCode.Mouse0) || InputManager.Instance.GetKeyUp(KeyCode.JoystickButton1) || Vector3.Distance(transform.position, pushObj.transform.position)> breakPushDis)
                {

                    pushObj.Break();
                    pushObj = null;
                    movement.isPush = false;
                    movement.isPull = false;
                }
            }
        }

        private void Climb()
        {
            Debug.Log("climb");
            if (movement.isClimbUp == true || climbNormal == Vector3.zero)
                return;
            panelNormal.x = climbNormal.x;
            panelNormal.y = climbNormal.y;
            panelNormal = panelNormal.normalized;
            //求出realNormal和right的夹角的余弦值
            float dot = Vector2.Dot(panelNormal, Vector2.right);
            float input = 0;
            if (Mathf.Abs(dot) < horizontalVerticalDivide)
            {
                //水平方向按键控制攀爬
                input = InputManager.Instance.GetHorizontal();
            }
            else
                input = InputManager.Instance.GetVertical();
            //Debug.Log("input:"+input);
            //Debug.Log("climbSpeed:" + climbSpeed);
            movement.moveVec = Quaternion.AngleAxis(90f, Vector3.forward) * panelNormal * climbSpeed * input;
            cc.Move(movement.moveVec * Time.fixedDeltaTime);
        }

        private void FallDown(float time)
        {
            movement.speedVertical = movement.speedVertical - gravity * time;
        }

        private void Jump(float speed = 0)
        {
            if (speed != 0)
                movement.speedVertical = speed;
            else
                movement.speedVertical = jumpSpeed;
        }

        /// <summary>
        /// 添加动画事件
        /// </summary>    /// <param name="animator"></param>
        /// <param name="clipName">动画名称</param>
        /// <param name="eventFunctionName">事件方法名称</param>
        /// <param name="time">添加事件时间。单位：秒</param>
        private void AddAnimationEvent(Animator animator, string clipName, string eventFunctionName, float time)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == clipName)
                {
                    AnimationEvent _event = new AnimationEvent();
                    _event.functionName = eventFunctionName;
                    if (time == -1)
                        time = clips[i].length;
                    _event.time = time;
                    clips[i].AddEvent(_event);
                    break;
                }
            }
            animator.Rebind();
        }

        private void ClimbUpFinish()
        {
            movement.isClimb = false;
            movement.isClimbUp = false;
            animator.applyRootMotion = false;
            rightHandTransform = null;
            cc.enabled = true;
            Debug.LogError("finish:" + transform.position);
        }

        public void SetRightHandClimbUpPoint(Transform trans)
        {
            Debug.LogError("SetRightHandClimbUpPoint");
            if (movement.isClimb == false)
                return;
            cc.enabled = false;
            Debug.LogError("rightHandTransform" + rightHandTransform.name);
            rightHandTransform = trans;
            animator.applyRootMotion = true;
            movement.isClimbUp = true;
            Debug.LogError("start" + transform.position);
            animator.MatchTarget(rightHandTransform.position, rightHandTransform.rotation,
                AvatarTarget.RightHand, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), 0f, 0f);
            Debug.LogError("rightHandTransform  1" + rightHandTransform.name);
        }

        private void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            Debug.LogError("OnCollisionStay");
            EvaluateCollision(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            Debug.LogError("OnCollisionExit");
            EvaluateExit(collision);
        }

        private void EvaluateCollision(Collision collision)
        {
            if (collision.gameObject.layer == climbableMask)
            {
                movement.isClimb = true;
                movement.isInAir = false;
                for (int i = 0; i < collision.contactCount; i++)
                {
                    climbNormal += collision.GetContact(i).normal;
                }
            }
        }

        private void EvaluateExit(Collision collision)
        {
            Debug.LogError("exit");
            if (collision.gameObject.layer == climbableMask)
            {
                movement.isClimb = false;
            }
        }

        private void ClearStastus()
        {
            climbNormal = Vector3.zero;
        }

        private void CheckOnGround()
        {
            if (cc.isGrounded)
                isInAir = false;
            else
                isInAir = true;
        }

        private void FixedUpdate()
        {
            if (!movement.isClimbUp)
            {
                if (movement.isClimb)
                    Climb();
                else
                    Move();
                Push();
            }
            movementAnimSetter.SetAnimatorInfo(movement);
            playerLifeAnimatorSetter.SetAnimatorInfo(player);
            ClearStastus();
        }
    }
}

