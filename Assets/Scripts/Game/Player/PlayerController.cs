using UnityEngine;

namespace AiLing
{
    //所有的玩家物理效果的接口都在此
    [RequireComponent(typeof(CharacterController),typeof(Rigidbody))]
    public class PlayerController : MonoSingleton<PlayerController>
    {
        public Movement movement;
        public float jumpSpeed = 6;
        public float horizontalSpeed = 10;
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
        }

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
                    movement.Jump();
                }
            }
            if (isInAir)
            {
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

