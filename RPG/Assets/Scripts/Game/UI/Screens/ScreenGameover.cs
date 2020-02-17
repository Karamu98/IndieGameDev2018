using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Game.UI.Screens
{
    public class ScreenGameover : Core.UI.Screen
    {
        public override string ScreenName { get { return "GameOver"; } }

        [SerializeField] Text m_endResultText;
        [SerializeField] Text m_potionCount;

        private void Awake()
        {
            m_endResultText.text = "You cleared " + (GameManager.s_CurrentFloor) + " floors and killed " + (GameManager.s_SessionKills) + " enemies...";
            m_potionCount.text = (GameManager.s_RevivePotions) + "x Revive's left";
        }
    }
}

