using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [RequireComponent(typeof(Rigidbody),typeof(CharacterController))]
    public class PushableObject : MonoBehaviour
    {
        private Rigidbody body;

        private CharacterController cc;

        private bool _bePushing;

        private Vector3 speed = new Vector3();

        private Movement pusher;
        public bool bePushing { get { return _bePushing; } }

        private void Start()
        {
            body = GetComponent<Rigidbody>();
            cc = GetComponent<CharacterController>();
            cc.enabled = false;
            body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            body.useGravity = true;
            SetKinematic(false);
            gameObject.layer = 8;
        }

        public void SetKinematic(bool bo)
        {
            body.isKinematic = bo;
        }

        public void Connect(Movement pusher)
        {
            _bePushing = true;
            SetKinematic(true);
            body.useGravity = false;
            cc.enabled = true;
            this.pusher = pusher;
        }

        public void Break()
        {
            _bePushing = false;
            SetKinematic(false);
            body.useGravity = true;
            cc.enabled = false;
            body.velocity = pusher.moveVec;
            pusher = null;
        }

        private void FixedUpdate()
        {
            if (_bePushing)
            {
                cc.Move(pusher.moveVec * Time.fixedDeltaTime);
            }
        }
    }
}

