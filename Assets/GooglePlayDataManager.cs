using System.Collections.Generic;
using System.Text;
using UnityEngine;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif

public class GooglePlayDataManager : MonoBehaviour
{
    public static GooglePlayDataManager Instance;

    internal List<int> unlockedWeapons = new();
    internal List<string> killstreaks = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Social.localUser.authenticated)
        {
            LoadGame();
        }
    }

    internal void SaveGame()
    {
#if UNITY_ANDROID
        SaveGameAndroid();
#elif UNITY_IOS
        SaveGameIOS();
#endif
    }

    internal void LoadGame()
    {
#if UNITY_ANDROID
        LoadGameAndroid();
#elif UNITY_IOS
        LoadGameIOS();
#endif
    }

#if UNITY_ANDROID
    private void SaveGameAndroid()
    {
        GameData gameData = new GameData
        {
            unlockedWeapons = unlockedWeapons,
            killstreaks = killstreaks
        };

        string dataToSave = JsonUtility.ToJson(gameData);
        byte[] data = Encoding.UTF8.GetBytes(dataToSave);

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            "game_save_data",
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
                    savedGameClient.CommitUpdate(game, update, data, (commitStatus, metadata) =>
                    {
                        if (commitStatus == SavedGameRequestStatus.Success)
                        {
                            Debug.Log("Game data saved successfully (Android)!");
                        }
                        else
                        {
                            Debug.Log("Failed to save game data (Android)");
                        }
                    });
                }
                else
                {
                    Debug.Log("Failed to open saved game (Android)");
                }
            });
    }

    private void LoadGameAndroid()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            "game_save_data",
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    savedGameClient.ReadBinaryData(game, (readStatus, data) =>
                    {
                        if (readStatus == SavedGameRequestStatus.Success)
                        {
                            string jsonData = Encoding.UTF8.GetString(data);
                            GameData loadedData = JsonUtility.FromJson<GameData>(jsonData);

                            if (loadedData != null)
                            {
                                unlockedWeapons = loadedData.unlockedWeapons;
                                killstreaks = loadedData.killstreaks;

                                Debug.Log("Game data loaded successfully (Android)!");
                            }
                            else
                            {
                                Debug.Log("Failed to parse game data (Android)");
                            }
                        }
                        else
                        {
                            Debug.Log("Failed to read game data (Android)");
                        }
                    });
                }
                else
                {
                    Debug.Log("Failed to open saved game for loading (Android)");
                }
            });
    }
#endif

#if UNITY_IOS
    private void SaveGameIOS()
    {
        GameData gameData = new GameData
        {
            unlockedWeapons = unlockedWeapons,
            killstreaks = killstreaks
        };

        string dataToSave = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString("game_save_data", dataToSave);
        PlayerPrefs.Save();

        Debug.Log("Game data saved successfully (iOS)!");
    }

    private void LoadGameIOS()
    {


        if (PlayerPrefs.HasKey("game_save_data"))
        {
            string jsonData = PlayerPrefs.GetString("game_save_data");
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonData);

            if (loadedData != null)
            {
                unlockedWeapons = loadedData.unlockedWeapons;
                killstreaks = loadedData.killstreaks;

                Debug.Log("Game data loaded successfully (iOS)!");
            }
            else
            {
                Debug.Log("Failed to parse game data (iOS)");
            }
        }
        else
        {
            Debug.Log("No saved game data found (iOS)");
        }
    }
#endif
}

[System.Serializable]
public class GameData
{
    public List<int> unlockedWeapons;
    public List<string> killstreaks;
}
