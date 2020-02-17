using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Game.UI
{ 
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] Image m_UI = default;
        [SerializeField] List<Sprite> m_images = default;
        [SerializeField] float m_speed = 1;

        float m_Counter = 0;
        int m_index = 0;
        private bool m_isActive = false;

        private void Update()
        {
            if (m_isActive)
            {
                m_Counter -= Time.deltaTime;
                if (m_Counter <= 0)
                {
                    if (m_UI.enabled == false)
                    {
                        m_UI.enabled = true;
                    }
                    m_Counter = m_speed;
                    m_UI.sprite = m_images[m_index];
                    m_index++;

                    if (m_index > m_images.Count - 1)
                    {
                        m_UI.enabled = false;
                        m_isActive = false;
                        m_index = 0;
                    }
                }
            }
        }

        public void Animate()
        {
            m_isActive = true;

        }
    }

}