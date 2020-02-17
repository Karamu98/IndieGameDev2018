using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Enemy : Character
    {
        // Audio
        [SerializeField] AudioClip m_audioClipDie;

        // Static Vars
        public static List<Enemy> s_AgentList = new List<Enemy>();


        // Member Vars
        public Vector3 PlayerLastPos;
        GameObject m_player;
        bool m_canSeePlayer = false;
        bool m_isPursuingPlayer = false;
        byte m_halffov = 55;



        protected override void Awake()
        {
            base.Awake();
        }

        public void SetPursue(bool a_newstate)
        {
            m_isPursuingPlayer = a_newstate;
        }

        protected virtual void Start()
        {
            // On start add ourselves to the brain list
            s_AgentList.Add(this);
            m_player = GameManager.s_Player.gameObject;
            m_audioSource.clip = m_audioClipDie;
        }

        protected override void Die()
        {
            m_audioSource.Play();
            GameManager.CharacterKilled(this);
            CharacterLocomotion.Shutdown();
            Destroy(gameObject);
        }

        protected virtual void Update()
        {
            //DrawFOV();
            UpdateAttacking();
            //CanSeePlayer();
        }

        void DrawFOV()
        {
            Quaternion leftRayRotation = Quaternion.AngleAxis(-m_halffov, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(m_halffov, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;

            Debug.DrawRay(transform.position, leftRayDirection * 4000, Color.red);
            Debug.DrawRay(transform.position, rightRayDirection * 4000, Color.red);
        }

        public Vector3 GetPosition()
        {
            return gameObject.transform.position;
        }

        public override bool CanMelee()
        {
            if (!base.CanMelee())
            {
                return false;
            }

            RaycastHit outHit;
            if (Physics.Raycast(transform.position, transform.forward, out outHit, GameManager.s_GridCellSize))
            {
                Character other = outHit.transform.gameObject.GetComponent<Character>();
                if (other == null)
                {
                    return false;
                }
                if (other.tag == "Player")
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanSeePlayer()
        {
            Vector3 line = Vector3.Normalize(m_player.transform.position - transform.position);

            float degree;

            if (line == transform.forward)
            {
                degree = 0;
            }
            else
            {
                // Finding the angle to the player
                degree = Vector3.Dot(line, transform.forward);
                degree = Mathf.Acos(degree);
                degree = degree * Mathf.Rad2Deg;
            }




            // If the player is within enemy FOV
            if (degree < m_halffov)
            {
                RaycastHit outHit;
                Debug.DrawRay(transform.position, line * 500);
                if (Physics.Raycast(transform.position, line, out outHit, 500))
                {
                    if (outHit.transform.gameObject == m_player)
                    {
                        PlayerLastPos = m_player.transform.position;
                        m_canSeePlayer = true;
                        return true;
                    }
                }
            }

            m_canSeePlayer = false;
            return false;
        }

        public bool isInPursuit()
        {
            if (PlayerLastPos == transform.position)
            {
                m_isPursuingPlayer = false;
                return false;
            }

            if (m_isPursuingPlayer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}