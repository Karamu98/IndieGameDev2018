using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ProjectileMagicMissile : Projectile
    {
        [SerializeField] ParticleSystem m_particleSystem = default;
        [SerializeField] AudioSource m_audioSource = default;
        [SerializeField] Light m_projectileLight = default;

        [SerializeField] float m_lingerTime = 1.0f;
        float m_lingerTimer = 0.0f;

        protected override void OnLaunch()
        {
            m_lingerTimer = m_lingerTime;
        }

        protected override void OnHit(Character other)
        {
            m_audioSource.Play();
            m_particleSystem.Stop();
            m_projectileLight.enabled = false;
        }

        private void Update()
        {
            if(HasCollided)
            {
                m_lingerTimer -= Time.deltaTime;
                if(m_lingerTimer <= 0.0f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}