using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

namespace Game
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] GameObject m_viewObj               = default;
        [SerializeField] protected float m_speed            = 0.5f;
        [SerializeField] protected float m_damage           = 0.0f;
        [SerializeField] CallbackCollider m_collider        = default;

        protected bool IsLaunched { get; private set; } = false;
        protected bool HasCollided { get; private set; } = false;

        Character m_caster;

        protected virtual void OnLaunch() { }
        protected virtual void OnHit(Character other) { }

        public void Init(Character owner)
        {
            m_caster = owner;
            m_collider.CallbackOnTriggerEnter += OnCollide;
        }

        public void Launch()
        {
            IsLaunched = true;
            OnLaunch();
        }

        public void SetDamage(int a_damage)
        {
            m_damage = a_damage;
        }

        private void FixedUpdate()
        {
            if(!HasCollided)
            {
                gameObject.transform.position += ((gameObject.transform.forward * m_speed) * Time.fixedDeltaTime);
            }
        }

        void OnCollide(Collider other)
        {
            if (other.gameObject == m_caster.gameObject)
            {
                return;
            }

            HasCollided = true;

            Character oth = other.gameObject.GetComponent<Character>();
            OnHit(oth);

            // So we can't hit anything else
            Destroy(m_collider);
            Destroy(m_viewObj);
        }
    }
}