using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Game
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] Text m_endResultText;
        [SerializeField] Text m_potionCount;

        private void Awake()
        {
            m_endResultText.text = "You cleared " + (GameManager.s_CurrentFloor) + " floors and killed " + (GameManager.s_SessionKills) + " enemies...";
            m_potionCount.text = (GameManager.s_RevivePotions) + "x Revive's left";
        }

        private void Update()
        {
            m_potionCount.text = (GameManager.s_RevivePotions) + "x Revive's left";
            //if (CrossPlatformInputManager.GetButtonDown("ContinueGame"))
            //{
            //    if (GameManager.iRevivePotions <= 0)
            //    {
            //        // Open shop to buy
            //        GameManager.IAPManager.BuyExtraLives();
            //    }
            //    else
            //    {
            //        // Consume and play game
            //        GameManager.iRevivePotions = GameManager.iRevivePotions - 1;

            //        GameManager.ContinueGame();


            //        this.gameObject.SetActive(false);

            //    }
            //}

            //if (CrossPlatformInputManager.GetButtonDown("EndGame"))
            //{
            //    GameManager.EndGame();
            //}
        }
    }
}

