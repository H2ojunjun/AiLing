using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [RequireComponent(typeof(BoxCollider))]
    public class ClimbUpPoint : MonoBehaviour
    {
        private BoxCollider _collider;
        private LayerMask _mask = 10;

        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (((1 << other.gameObject.layer) & (1 << _mask)) != 0)
            //{
            //    PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            //    pc.SetRightHandClimbUpPoint(transform);
            //}
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

