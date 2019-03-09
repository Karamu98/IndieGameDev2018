using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using GooglePlayGames;

public class CS_Menu : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
        if(CrossPlatformInputManager.GetButtonDown("Play"))
        {
            CS_GameManager.NewGame();
        }

        if (CrossPlatformInputManager.GetButtonDown("Leaderboard"))
        {
            CS_PlayServices.ShowLeaderboard();
        }
	}
}
