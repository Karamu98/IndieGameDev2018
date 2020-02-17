using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Character : MonoBehaviour
    {
        // Audio
        protected AudioSource m_audioSource;

        // Movement
        public Locomotion CharacterLocomotion { get; protected set; } = new Locomotion();

        // Combat
        [SerializeField] int m_maxHealth = 100;
        int m_currentHealth = 100;
        int m_tempHealth = 0;

        [SerializeField] int m_damage = 10;

        [SerializeField]  float m_meleeCooldown = 1;
        float m_meleeTimer = 0;

        [SerializeField] float m_magicCooldown = 5;
        float m_magicTimer = 0;

        [SerializeField] GameObject m_baseSpell;

        // State control
        protected bool m_isFreeLooking = false;
        protected bool m_isAttacking = false;
        protected bool m_isGameActive = true;


        protected virtual void Awake()
        {
            GameManager.AddCharacter(this);
            m_audioSource = GetComponent<AudioSource>();
        }

        public void SetDamage(int a_newDamage)
        {
            m_damage = a_newDamage;
        }

        public void SetHealth(int a_newHealth)
        {
            m_currentHealth = a_newHealth;
        }

        public void FullHeal()
        {
            m_currentHealth = m_maxHealth;
        }

        public virtual void TakeDamage(int a_amount)
        {
            m_currentHealth = m_currentHealth - a_amount;

            if (m_currentHealth <= 0)
            {
                Die();
            }
        }

        public void MeleeAttack()
        {
            if (m_meleeTimer > 0)
            {
                return;
            }

            m_meleeTimer = m_meleeCooldown;
            RaycastHit outHit;
            if (Physics.Raycast(transform.position, transform.forward, out outHit, GameManager.s_GridCellSize))
            {
                Character other = outHit.transform.gameObject.GetComponent<Character>();
                if (other != null)
                {
                    Debug.Log("Agent: " + this.gameObject.name + " deals " + m_damage + " damage to " + other.gameObject.name);
                    other.TakeDamage(m_damage);
                }
            }
        }

        public bool MagicAttack()
        {
            if (m_magicTimer > 0)
            {
                return false;
            }
            m_magicTimer = m_magicCooldown;
            GameObject spell = Instantiate(m_baseSpell, transform.position, transform.rotation);
            Projectile proj = spell.GetComponent<Projectile>();
            proj.Init(this);
            proj.SetDamage(m_damage);
            proj.Launch();

            return true;
        }

        public int GetHealth()
        {
            return m_currentHealth;
        }

        protected abstract void Die();

        public void OnGameStart()
        {
            m_isGameActive = true;
        }

        public void OnGameEnd()
        {
            m_isGameActive = false;
            m_isFreeLooking = false;

            CharacterLocomotion.Shutdown();
        }

        public virtual bool CanMelee()
        {
            if (m_meleeTimer > 0)
            {
                return false;
            }
            return true;
        }

        public bool CanMagic()
        {
            if (m_magicTimer > 0)
            {
                return false;
            }
            return true;
        }

        public void UpdateAttacking()
        {
            if (m_meleeTimer > 0)
            {
                m_meleeTimer -= Time.deltaTime;
            }

            if (m_magicTimer > 0)
            {
                m_magicTimer -= Time.deltaTime;
            }
        }

        [System.Serializable]
        public class Locomotion
        {
            [SerializeField] float m_speed = 12;
            [SerializeField] float m_rotSpeed = 12;

            Vector3 m_startPos;
            Vector3 m_destPos;
            float m_moveProgress;
            public bool IsMoving { get; private set; }

            Quaternion m_startRot;
            Quaternion m_destRot;
            float m_rotProgress;
            public bool IsRotating { get; private set; }

            public Vector2Int GridPos { get; private set; } = new Vector2Int();

            GameObject m_viewObj;

            public void Init(GameObject viewObj)
            {
                m_viewObj = viewObj;
                m_startPos = m_viewObj.transform.position;
                m_startRot = m_viewObj.transform.rotation;

                GridPos = GameManager.GetGridPos(m_startPos);
            }

            public void Shutdown()
            {
                IsMoving = false;
                IsRotating = false;

                if(IsMoving)
                {
                    GameManager.ClearSpace(m_destPos);
                }
                else
                {
                    GameManager.ClearSpace(m_startPos);
                }                
            }

            public void Process()
            {
                if(IsMoving)
                {
                    m_moveProgress = Mathf.Clamp01(m_moveProgress + (m_speed * Time.deltaTime));
                    m_viewObj.transform.position = Vector3.Lerp(m_startPos, m_destPos, m_moveProgress);

                    if (m_moveProgress == 1.0f)
                    {
                        m_viewObj.transform.position = Core.Utilities.Math.RoundToInt(m_destPos);
                        IsMoving = false;
                        m_startPos = m_destPos;
                        m_moveProgress = 0.0f;
                    }
                }

                if(IsRotating)
                {
                    m_rotProgress = Mathf.Clamp01(m_rotProgress + (m_rotSpeed * Time.deltaTime));
                    m_viewObj.transform.rotation = Quaternion.Lerp(m_startRot, m_destRot, m_rotProgress);

                    if(m_rotProgress == 1.0f)
                    {
                        m_viewObj.transform.rotation = Quaternion.Euler(Core.Utilities.Math.RoundToInt(m_destRot.eulerAngles));
                        IsRotating = false;
                        m_startRot = m_destRot;
                        m_rotProgress = 0.0f;
                    }
                }
            }

            public void Move(Direction dirToMove)
            {
                if (IsMoving)
                {
                    return;
                }

                Vector3 axisToMove = AxisFromDirection(dirToMove);
                Vector3 dest = m_viewObj.transform.position + (axisToMove * GameManager.s_GridCellSize);

                if (!GameManager.IsClear(dest))
                {
                    return;
                }

                if (GameManager.TakeSpace(dest))
                {
                    m_destPos = dest;
                    GameManager.ClearSpace(m_startPos);
                    IsMoving = true;
                }
            }

            public void Turn(Direction dirToLook)
            {
                if (IsRotating)
                {
                    return;
                }

                m_destRot = Quaternion.LookRotation(AxisFromDirection(dirToLook), Vector3.up);
                IsRotating = true;
            }

            Vector3 AxisFromDirection(Direction dir)
            {
                switch (dir)
                {
                    case Direction.FORWARD:
                        {
                            return m_viewObj.transform.forward;
                        }
                    case Direction.LEFT:
                        {
                            return -m_viewObj.transform.right;
                        }
                    case Direction.RIGHT:
                        {
                            return m_viewObj.transform.right;
                        }
                    case Direction.BACKWARDS:
                        {
                            return -m_viewObj.transform.forward;
                        }
                }
                return Vector3.zero;
            }
        }
    }
}