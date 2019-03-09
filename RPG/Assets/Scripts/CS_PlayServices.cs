using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using UnityEngine;

public class CS_PlayServices : MonoBehaviour
{
    public static CS_PlayServices instance = null;
    public static bool bIsSignedIn;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        InitialiseGooglePlay();
    }

    private static void InitialiseGooglePlay()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();


        PlayGamesPlatform.Instance.Authenticate((bool success) =>
        {
            if (success)
            {
                bIsSignedIn = true;
            }
            else
            {
                Debug.Log("Failed to log in");
                bIsSignedIn = false;
            }
            // handle success or failure
        });
    }


    #region Leaderboards

    public static void AddScoreToLeaderboard(string a_leaderboardID, long a_score)
    {
        if(bIsSignedIn)
        {
            Social.ReportScore(a_score, a_leaderboardID, (bool Success) =>
            {
                if(!Success)
                {
                    bIsSignedIn = false;
                }
            });
        }
    }

    public static void ShowLeaderboard()
    {
        if(bIsSignedIn)
        {
            Social.ShowLeaderboardUI();
        }
    }


    #endregion

}
