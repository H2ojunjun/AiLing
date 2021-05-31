using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;

namespace AiLing
{
    public class Button : MonoBehaviour
    {
        [LabelText("碰撞mask")]
        public LayerMask mask = 1 << 12;
        [LabelText("按钮方向")]
        public Vector3 buttonVec;
        [LabelText("是否回弹")]
        public bool isBack;
        [LabelText("角度范围")]
        public float angle;
        [LabelText("位移范围")]
        public float distance;
        [LabelText("位移时长")]
        public float time;
        public TweenCallback completeCallBack;
        private Vector3 _originPos;
        private bool _isMoving = false;
        private void Awake()
        {
            _originPos = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            DebugHelper.Log("OnCollisionEnter  " + other.gameObject);
            if (_isMoving)
                return;
            if (((1 << other.gameObject.layer) & mask) != 0)
            {
                Vector3 normal = transform.position - other.transform.position;
                if (Vector3.Dot(normal.normalized, buttonVec.normalized) > Mathf.Cos(angle))
                {
                    _isMoving = true;
                    transform.DOMove(_originPos + buttonVec * distance, time, false).onComplete = ()=> {
                        if (isBack)
                            transform.DOMove(_originPos, time, false).onComplete = ()=> {
                                _isMoving = false;
                            };
                        else
                            _isMoving = false;
                    };
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & mask) != 0)
            {
                //transform.DOMove(_originPos, time, false);
            }
        }
    }
}

