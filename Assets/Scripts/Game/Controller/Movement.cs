using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    private const float RUN_SPEED_MIN = 10.0F;

    private float _speedHorizontal;
    private float _speedVertical;
    private float _gravity = 10;
    private float _jumpSpeed { get { return owner.jumpSpeed; } }

    public float speedHorizontal 
    { 
        get 
        {   return _speedHorizontal; }
        set 
        {
            if (!canWalk)
            {
                _speedHorizontal = 0;
                isRun = false;
            }
            else if (!canRun)
            {
                _speedHorizontal = Mathf.Min(value, RUN_SPEED_MIN);
                isRun = false;
            }
            if (value > RUN_SPEED_MIN)
            {
                isRun = true;
            }

            else if (value > 0)
                isWalk = true;
            else
                isWalk = false;
            _speedHorizontal = value;
        } 
    }

    public float speedVertical
    {
        get
        {   return _speedVertical; }
        set
        {
            _speedVertical = value;
        }
    }

    public bool canWalk = true;
    public bool canRun = true;
    public bool canJump = true;

    public PlayerController owner;
    public bool isWalk;
    public bool isRun;
    public bool isInAir;
    
    //总速度的模
    public float moveSpeed { get { return moveVec.magnitude; } }
    //速度的向量
    public Vector3 moveVec { get { return Vector3.right * speedHorizontal + Vector3.up * speedVertical; } }

    public Movement(PlayerController owner)
    {
        this.owner = owner;
        //Rigidbody body = owner.GetComponent<Rigidbody>();
        //if (body == null)
        //{
        //    Debug.LogError("cant find rigidbody");
        //}
        //else
        //{
        //    _gravity = body.
        //}
    }

    //public void FallingDown(Rigidbody body, float time)
    //{
    //    speedVertical = speedVertical - _gravity * time;
    //    body.velocity -= Vector3.up * _gravity * time;
    //}

    public void FallingDown(float time)
    {
        speedVertical = speedVertical - _gravity * time;
    }

    //public void Jump(Rigidbody body, float speed = 0)
    //{
    //    if (speed != 0)
    //        speedVertical = speed;
    //    else
    //        speedVertical = _jumpSpeed;
    //    body.velocity = new Vector3(0, speedVertical, 0);
    //}

    public void Jump(float speed = 0)
    {
        if (speed != 0)
            speedVertical = speed;
        else
            speedVertical = _jumpSpeed;
    }
}
