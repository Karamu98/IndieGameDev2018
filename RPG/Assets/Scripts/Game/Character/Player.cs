using UnityEngine;
using UnityEngine.UI;


namespace Game
{
    public enum Direction
    {
        FORWARD,
        BACKWARDS,
        LEFT,
        RIGHT
    }

    public class Player : Character
    {
        [SerializeField] GameObject m_mainCamera;
        [SerializeField] GameObject m_mapCamera;
        [SerializeField] UI.SpriteAnimator m_beenHitAnim;
        [SerializeField] RawImage[] m_miniImages;
        [SerializeField] GameObject[] m_UIElements;
        [SerializeField] GameObject m_deathUI;

        // Audio
        [SerializeField] AudioClip m_audioClipHeal;
        [SerializeField] AudioClip m_audioClipHurt;
        [SerializeField] AudioClip m_audioClipSpellCast;
        [SerializeField] AudioClip m_audioClipSwipe;

        // Use this for initialization
        void Start()
        {
            GameManager.SetPlayer(this);
            // Start the player at the correct height
            float y = (-GameManager.s_GridCellSize * 0.5f);

            transform.position = new Vector3(transform.position.x, y, transform.position.z);

            CharacterLocomotion.Init(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_isGameActive)
            {
                /// Attacking
                HandleAttacking();

                CharacterLocomotion.Process();

                /// Minimap toggle
                Minimap();
            }
        }

        private void HandleAttacking()
        {
            UpdateAttacking();

            //if (CrossPlatformInputManager.GetButtonDown("Melee"))
            //{
            //    if (CanMelee())
            //    {
            //        MeleeAttack();
            //        audioSource.clip = m_audioClipSwipe;
            //        audioSource.Play();
            //    }
            //}

            //if (CrossPlatformInputManager.GetButtonDown("Magic"))
            //{
            //    if (MagicAttack())
            //    {
            //        audioSource.clip = m_audioClipSpellCast;
            //        audioSource.Play();
            //    }
            //}
        }

        public override void TakeDamage(int a_amount)
        {
            m_beenHitAnim.Animate();
            m_audioSource.clip = m_audioClipHurt;
            m_audioSource.Play();
            base.TakeDamage(a_amount);
        }

        protected override void Die()
        {
            // Remove all UI
            for (int i = 0; i < 2; i++)
            {
                Destroy(m_miniImages[i]);
            }

            for (int i = 0; i < m_UIElements.Length; i++)
            {
                Destroy(m_UIElements[i]);
            }

            // Display dead UI
            m_deathUI.SetActive(true);

            GameManager.PlayerDied();
        }

        private void Minimap()
        {
            //if (CrossPlatformInputManager.GetButton("Minimap"))
            //{
            //    m_mainCamera.SetActive(false);
            //    m_mapCamera.SetActive(true);

            //    for (int i = 0; i < 2; i++)
            //    {
            //        m_miniImages[i].color = new Color(m_miniImages[i].color.r, m_miniImages[i].color.g, m_miniImages[i].color.b, 0);
            //    }

            //    for (int i = 0; i < m_UIElements.Length; i++)
            //    {
            //        m_UIElements[i].SetActive(false);
            //    }


            //}
            //if (CrossPlatformInputManager.GetButtonUp("Minimap"))
            //{
            //    m_mainCamera.SetActive(true);
            //    m_mapCamera.SetActive(false);

            //    for (int i = 0; i < 2; i++)
            //    {
            //        m_miniImages[i].color = new Color(m_miniImages[i].color.r, m_miniImages[i].color.g, m_miniImages[i].color.b, 1);
            //    }

            //    for (int i = 0; i < m_UIElements.Length; i++)
            //    {
            //        m_UIElements[i].SetActive(true);
            //    }
            //}
        }
    }
}