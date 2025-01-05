using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;
using UnityEngine.Networking;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#elif UNITY_IOS
using GameKit;
#endif

public class UnityAuth : MonoBehaviour
{
    public static UnityAuth Instance;


    private string leaderboardID = "CgkIh4v65MgXEAIQAQ"; // Replace with your actual leaderboard ID

    public Sprite profileImage;

    void Awake()
    {
        // Make this object persistent across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.LogError("Authenticating------------|||||----------------------");
        #if UNITY_ANDROID
        PlayGamesPlatform.Activate();
        #endif
        AuthenticateUser();
    }

    // Authenticate the user
    void AuthenticateUser()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games successful.");

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Debug.Log("Authorization code: " + code);
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                });
                if (!PlayerPrefs.HasKey("PlayerName"))
                {
                    PlayerPrefs.SetString("PlayerName", PlayGamesPlatform.Instance.GetUserDisplayName());
                }

                StartCoroutine(LoadSpriteFromURL(PlayGamesPlatform.Instance.GetUserImageUrl()));
            }
            else
            {
                Debug.Log("Login Unsuccessful" + success);
            }
        });

#elif UNITY_IOS
        AuthenticateWithAppleGameCenter();
#endif
    }

#if UNITY_IOS
    async void AuthenticateWithAppleGameCenter()
    {
        if (!GKLocalPlayer.Local.IsAuthenticated)
        {
            try
            {
                await GKLocalPlayer.Authenticate();
                Debug.Log("Apple Game Center Authentication Successful");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Apple Game Center Authentication Failed: " + ex.Message);
            }
        }
    }
#endif

    // Submit score to leaderboard
    public void SubmitScore()
    {
#if UNITY_ANDROID
        Social.LoadScores(leaderboardID, result =>
        {
            long score = 0;
            for (int i = 0; i < result.Length; i++)
            {
                if(result[i].userID== PlayGamesPlatform.Instance.localUser.id)
                {
                    score = result[i].value;
                }
            }
            Social.ReportScore(score+1, leaderboardID, success =>
            {
                if (success)
                {
                    Debug.Log("Score submitted successfully to Google Play Games.");
                }
                else
                {
                    Debug.Log("Failed to submit score to Google Play Games.");
                }
            });
        });
       

#elif UNITY_IOS
        var leaderboardScore = new GKLeaderboardScore
        {
            Context = 0, // Set to any additional context value
            Value += 1// The score to be submitted
        };

        GKLocalPlayer.Local.SubmitScore(leaderboardScore, leaderboardID, (error) =>
        {
            if (error == null)
            {
                Debug.Log("Score submitted successfully to Apple Game Center.");
            }
            else
            {
                Debug.LogError("Failed to submit score to Apple Game Center: " + error.LocalizedDescription);
            }
        });
#endif
    }

    // Show the leaderboard UI
    public void ShowLeaderboard()
    {
#if UNITY_ANDROID
        //Social.ShowLeaderboardUI();
        PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardID);
#elif UNITY_IOS
        // Showing Game Center leaderboard
        var leaderboard = new GKLeaderboard
        {
            Identifier = leaderboardID
        };
        leaderboard.LoadScores((scores, error) =>
        {
            if (error == null)
            {
                // Display Game Center UI here
                GKGameCenterViewController viewController = new GKGameCenterViewController
                {
                    ViewState = GKGameCenterViewControllerState.Leaderboards
                };
                UnityEngine.iOS.Device.SetNoBackupFlag(viewController.ToString()); // Example to show
                Debug.Log("Showing Game Center Leaderboard UI");
            }
            else
            {
                Debug.LogError("Failed to load leaderboard scores: " + error.LocalizedDescription);
            }
        });
#endif
    }

    // Example of updating score (e.g., distance) and persisting it
    IEnumerator LoadSpriteFromURL(string url)
    {
        // Start downloading the image from the URL
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
        yield return webRequest.SendWebRequest();

        // Check if there were errors
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error downloading image: " + webRequest.error);
        }
        else
        {
            // Get the texture from the download
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

            // Create a new sprite from the texture
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            profileImage = newSprite;
        }
    }
}
