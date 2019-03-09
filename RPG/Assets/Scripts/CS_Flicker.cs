using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Flicker : MonoBehaviour
{
    [SerializeField] Light SourceLight;

    [SerializeField] private float maxIntensity = 3;
    [SerializeField] private float minIntensity = 1;
    [SerializeField] private float flickerSpeed = 0.035f;
    [SerializeField] private bool bSoft = false;

    private float timer = 0;
    float newIntensity;

    // Update is called once per frame
    void Update ()
    {
        if(bSoft)
        {
            if(timer <= 0)
            {
                newIntensity = Random.Range(minIntensity, maxIntensity);
                timer = flickerSpeed;
            }

            SourceLight.intensity = Mathf.Lerp(SourceLight.intensity, newIntensity, Time.deltaTime * (flickerSpeed * 100));
            timer = timer - Time.deltaTime;
        }
        else
        {
            if (timer <= 0)
            {
                SourceLight.intensity = Random.Range(minIntensity, maxIntensity);
                timer = flickerSpeed;
            }
            else
            {
                timer = timer - Time.deltaTime;
            }
        }
	}
}
