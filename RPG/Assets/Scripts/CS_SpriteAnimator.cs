using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CS_SpriteAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> images;
    [SerializeField] float fSpeed = 1;

    Image UI;

    float fCounter = 0;
    int index = 0;
    private bool bActive = false;

    private void Awake()
    {
        UI = GetComponent<Image>();
    }

    private void Update()
    {
        if(bActive)
        {
            fCounter -= Time.deltaTime;
            if(fCounter <= 0)
            {
                if (UI.enabled == false)
                {
                    UI.enabled = true;
                }
                fCounter = fSpeed;
                UI.sprite = images[index];
                index++;

                if (index > images.Count - 1)
                {
                    UI.enabled = false;
                    bActive = false;
                    index = 0;
                }
            }

            


        }
    }


    public void Animate()
    {
        bActive = true;      

    }
}
