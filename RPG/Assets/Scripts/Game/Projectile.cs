using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] AudioClip m_collision;
        [SerializeField] float m_speed = 0.5f;
        [SerializeField] int m_damage;
        [SerializeField] GameObject m_sphere;

        AudioSource m_source;
        bool m_shouldLaunch = false;
        bool m_hasCollided;
        float m_maxIntensity;
        bool m_isDying;

        Character m_caster;

        public void SetCaster(Character owner)
        {
            m_caster = owner;
        }

        public void Launch()
        {
            m_source = GetComponent<AudioSource>();
            m_source.clip = m_collision;
            m_shouldLaunch = true;
            GetComponent<SphereCollider>().enabled = true;
        }

        public void SetDamage(int a_damage)
        {
            m_damage = a_damage;
        }

        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject == m_caster.gameObject)
            {
                return;
            }

            m_source.Play();
            m_hasCollided = true;
            Destroy(GetComponent<CS_Flicker>());
            Destroy(GetComponent<SphereCollider>());
            Destroy(m_sphere);
            Character oth = other.gameObject.GetComponent<Character>();
            if (oth != null)
            {
                oth.TakeDamage(Mathf.RoundToInt(m_damage * 0.5f));
            }



            m_maxIntensity = GetComponent<Light>().intensity + 1;
        }

        private void Update()
        {
            if (!m_shouldLaunch)
            {
                return;
            }

            if (m_hasCollided)
            {
                if (m_isDying)
                {
                    if (GetComponent<Light>().intensity <= 0.2f)
                    {
                        Destroy(gameObject);
                    }

                    GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, 0, Time.deltaTime * 10);
                }
                else
                {
                    if (GetComponent<Light>().intensity >= m_maxIntensity - 0.2)
                    {
                        m_isDying = true;
                    }

                    GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, m_maxIntensity, Time.deltaTime * 7);
                }
            }
            else
            {
                transform.position = transform.position + (transform.forward * m_speed);
            }
        }
    }
}