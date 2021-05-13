using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [RequireComponent(typeof(Rigidbody),typeof(CharacterController))]
    public class PushableObject : MonoBehaviour
    {
        private Rigidbody _body;

        private CharacterController _cc;

        private bool _bePushing;

        private Movement _pusher;
        public bool bePushing { get { return _bePushing; } }

        private void Start()
        {
            _body = GetComponent<Rigidbody>();
            _cc = GetComponent<CharacterController>();
            _cc.enabled = false;
            _body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            _body.useGravity = true;
            SetKinematic(false);
            gameObject.layer = 8;
        }

        public void SetKinematic(bool bo)
        {
            _body.isKinematic = bo;
        }

        public void Connect(Movement pusher)
        {
            _bePushing = true;
            SetKinematic(true);
            _body.useGravity = false;
            _cc.enabled = true;
            this._pusher = pusher;
        }

        public void Break()
        {
            _bePushing = false;
            SetKinematic(false);
            _body.useGravity = true;
            _cc.enabled = false;
            _body.velocity = _pusher.moveVec;
            _pusher = null;
        }

        private void FixedUpdate()
        {
            if (_bePushing)
            {
                _cc.Move(_pusher.moveVec * Time.fixedDeltaTime);
            }
        }
    }
}

