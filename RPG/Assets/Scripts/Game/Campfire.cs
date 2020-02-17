using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    [SerializeField] Light m_sourceLight;

    [SerializeField] float m_maxIntensity = 3;
    [SerializeField] float m_minIntensity = 1;
    [SerializeField] float m_flickerSpeed = 0.035f;

    float m_timer = 0;

    // Update is called once per frame
    void Update ()
    {
        m_timer -= Time.deltaTime;

        if (m_timer <= 0)
        {
            m_sourceLight.intensity = Random.Range(m_minIntensity, m_maxIntensity);
            m_timer = m_flickerSpeed;
        }
	}
}
