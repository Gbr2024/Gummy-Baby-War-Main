using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;
using UnityEngine.Networking;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class UnityAuth : MonoBehaviour
{
    public static UnityAuth Instance;
    private string leaderboardID;


    public Sprite profileImage;

    void Awake()
    {
        #if UNITY_ANDROID
            leaderboardID = "CgkIh4v65MgXEAIQAQ"; // Replace with your actual leaderboard ID
        #elif UNITY_IOS
            leaderboardID = "com.gummybabywarleaderboard"; // Replace with your actual leaderboard ID
        #endif
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
        Social.localUser.Authenticate(ProcessAuthentication);
#endif
    }

#if UNITY_IOS
    void ProcessAuthentication(bool success)
    {
        if (success)
        {
            var PlayerName = Social.localUser.userName;
            if (!PlayerPrefs.HasKey("PlayerName"))
            {
                PlayerPrefs.SetString("PlayerName", PlayerName);
            }
            //SignIn();
            //SignInAndCheck();
            Debug.Log("Authenticated, checking achievements");
        }
        else
        {

            Debug.Log("Unable To Authenticcate");
        }

    }
#endif

    // Submit score to leaderboard
    public void SubmitScore()
    {
        Social.LoadScores(leaderboardID, result =>
        {
            long score = 0;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].userID == Social.localUser.id)
                {
                    score = result[i].value;
                }
            }
            Social.ReportScore(score + 1, leaderboardID, success =>
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
    }

    // Show the leaderboard UI
    public void ShowLeaderboard()
    {
#if UNITY_ANDROID
        //Social.ShowLeaderboardUI();
        PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardID);
#elif UNITY_IOS
        // Showing Game Center leaderboard
        Social.ShowLeaderboardUI();
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
