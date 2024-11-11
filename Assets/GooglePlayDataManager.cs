using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GooglePlayDataManager : MonoBehaviour
{
    public static GooglePlayDataManager Instance;

    internal List<int> unlockedWeapons = new();
    internal List<string> killstreaks = new();

    private void Awake()
    {
        if(Instance==null)
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
        if(Social.localUser.authenticated)
        {
            LoadGame();
        }
    }

    internal void SaveGame()
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
                            Debug.Log("Game data saved successfully!");
                        }
                        else
                        {
                            Debug.Log("Failed to save game data");
                        }
                    });
                }
                else
                {
                    Debug.Log("Failed to open saved game");
                }
            });
    }

    internal void LoadGame()
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

                                Debug.Log("Game data loaded successfully!");
                            // You can now use `unlockedWeapons` and `killstreaks` as needed.
                        }
                            else
                            {
                                Debug.Log("Failed to parse game data");
                            }
                        }
                        else
                        {
                            Debug.Log("Failed to read game data");
                        }
                    });
                }
                else
                {
                    Debug.Log("Failed to open saved game for loading");
                }
            });
    }
}

[System.Serializable]
public class GameData
{
    public List<int> unlockedWeapons;
    public List<string> killstreaks;
}