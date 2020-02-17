using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Screens
{
    public class ScreenMainMenu : Core.UI.Screen
    {
        public override string ScreenName { get { return "MainMenu"; } }

        public void OnPlayPressed()
        {
            GameManager.NewGame();
        }

        public void OnLeaderboardPressed()
        {
            Debug.Log("Not implemented.");
        }
    }
}