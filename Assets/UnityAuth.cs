using UnityEngine;
using UnityEngine.SocialPlatforms;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class UnityAuth : MonoBehaviour
{
//    void Start()
//    {
//        AuthenticateUser();
//    }

//    void AuthenticateUser()
//    {
//#if UNITY_ANDROID
//        // Google Play Games Authentication
//        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
//            .RequestEmail()   // Request email (optional)
//            .Build();

//        PlayGamesPlatform.InitializeInstance(config);
//        PlayGamesPlatform.DebugLogEnabled = true;  // Enable debugging
//        PlayGamesPlatform.Activate();  // Activate Google Play Games

//        Social.localUser.Authenticate(success =>
//        {
//            if (success)
//            {
//                Debug.Log("Google Play Login Successful!");
//                Debug.Log("User ID: " + Social.localUser.id);
//                Debug.Log("User Name: " + Social.localUser.userName);
//            }
//            else
//            {
//                Debug.Log("Google Play Login Failed!");
//            }
//        });

//#elif UNITY_IOS
//        // Apple Game Center Authentication
//        if (!Social.localUser.authenticated)
//        {
//            Social.localUser.Authenticate(success =>
//            {
//                if (success)
//                {
//                    Debug.Log("Game Center Login Successful!");
//                    Debug.Log("User ID: " + Social.localUser.id);
//                    Debug.Log("User Name: " + Social.localUser.userName);
//                }
//                else
//                {
//                    Debug.Log("Game Center Login Failed!");
//                }
//            });
//        }
//#endif
//    }

//    public void SignOut()
//    {
//#if UNITY_ANDROID
//        PlayGamesPlatform.Instance.SignOut();
//        Debug.Log("Signed out from Google Play.");

//#elif UNITY_IOS
//        Debug.Log("Apple Game Center does not support sign-out.");
//#endif
//    }
}
