using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities
{
    [RequireComponent(typeof(Collider))]
    public class CallbackCollider : MonoBehaviour
    {
        [SerializeField] Collider m_collider = default;
        public Collider Collider { get { return m_collider; } }
        public Action<Collider> CallbackOnTriggerEnter;
        public Action<Collider> CallbackOnTriggerStay;
        public Action<Collider> CallbackOnTriggerLeave;

        private void OnTriggerEnter(Collider other)
        {
            CallbackOnTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            CallbackOnTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            CallbackOnTriggerLeave?.Invoke(other);
        }

    }
}

