using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using UnityEngine.UI;
using UnityEngine;

public class CS_GameOverUI : MonoBehaviour
{
    [SerializeField] private Text endResultText;
    [SerializeField] private Text potionCount;

    private void Awake()
    {
        endResultText.text = "You cleared " + (CS_GameManager.iCurrentFloor) + " floors and killed " + (CS_GameManager.iSessionKills) + " enemies...";
        potionCount.text = (CS_GameManager.iRevivePotions) + "x Revive's left";
    }

    private void Update()
    {
        potionCount.text = (CS_GameManager.iRevivePotions) + "x Revive's left";
        if (CrossPlatformInputManager.GetButtonDown("ContinueGame"))
        {
            if(CS_GameManager.iRevivePotions <= 0)
            {
                // Open shop to buy
                CS_GameManager.IAPManager.BuyExtraLives();
            }
            else
            {
                // Consume and play game
                CS_GameManager.iRevivePotions = CS_GameManager.iRevivePotions - 1;

                CS_GameManager.ContinueGame();


                this.gameObject.SetActive(false);

            }
        }

        if(CrossPlatformInputManager.GetButtonDown("EndGame"))
        {
            CS_GameManager.EndGame();
        }
    }
}
