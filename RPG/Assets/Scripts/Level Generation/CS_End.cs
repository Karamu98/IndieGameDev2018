using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_End : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == CS_GameManager.player.gameObject)
        {
            CS_GameManager.LevelComplete();
        }
        else
        {
        }
    }
}
