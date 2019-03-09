using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine;

public class CS_ADManager : MonoBehaviour
{
    bool bAdShown = false;
    private void Awake()
    {
        bAdShown = false;
    }

    private void Update()
    {
        if(!bAdShown)
        {
            if (Advertisement.IsReady("video"))
            {
                Advertisement.Show("video");
                bAdShown = true;
            }

        }
        else
        {
            if(!Advertisement.isShowing)
            {
                if(CS_GameManager.bGameEnd)
                {
                    SceneManager.LoadScene(0);
                }
                else
                {
                    SceneManager.LoadScene(1);
                }
                
            }
        }


    }


}
