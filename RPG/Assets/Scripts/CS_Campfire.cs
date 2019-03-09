using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Campfire : MonoBehaviour
{
    [SerializeField] Light SourceLight;

    [SerializeField] private float maxIntensity = 3;
    [SerializeField] private float minIntensity = 1;
    [SerializeField] private float flickerSpeed = 0.035f;

    private float timer = 0;

    // Update is called once per frame
    void Update ()
    {
		if(timer <= 0)
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
