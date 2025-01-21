using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using System.Net;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using UnityEngine.Networking;
using LitJson;
using System.Text;
using System;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    private const string MultiplayApiBaseUrl = "https://services.api.unity.com/multiplay/servers/v1";
    [SerializeField] private string projectId; // Set in Unity Inspector
    [SerializeField] private string environmentId;
    [SerializeField] private string apiKey;


    //Lobby Settings
    const int MaxPlayers = 10, MinimumPlayerToStartGame = 2;
    private readonly string KEY_SERVER_ADDRESS = "ServerAddress";
    private readonly string KEY_SERVER_PORT = "ServerPort";
    private readonly string KEY_REDCOLOR_CODE = "RedColor";
    private readonly string KEY_BLUECOLOR_CODE = "BlueColor";
    private readonly string KEY_SKYBOX_Code = "Skybox";
    private readonly string KEY_LEVEL_CODE = "Level";
    private readonly string KEY_AI_NAME = "AIname";
    private readonly string KEYSTORM = "EnableStorm";

    internal Lobby JoinedLobby;
    internal bool GameSceneHasLoaded = false;
    bool isLobbyHost = false;
    internal bool GameHasStarted = false;
    private Coroutine waitCor;

    [System.Serializable]
    public class MachineSpec
    {
        public DateTime contractEndDate { get; set; }
        public DateTime contractStartDate { get; set; }
        public int cpuCores { get; set; }
        public string cpuDetail { get; set; }
        public string cpuName { get; set; }
        public string cpuShortname { get; set; }
        public int cpuSpeed { get; set; }
        public Int64 memory { get; set; }
    }

    public class Machines
    {
        public int buildConfigurationID { get; set; }
        public string buildConfigurationName { get; set; }
        public string buildName { get; set; }
        public int cpuLimit { get; set; }
        public bool deleted { get; set; }
        public string fleetID { get; set; }
        public string fleetName { get; set; }
        public string hardwareType { get; set; }
        public int holdExpiresAt { get; set; }
        public int id { get; set; }
        public string ip { get; set; }
        public int locationID { get; set; }
        public string locationName { get; set; }
        public int machineID { get; set; }
        public string machineName { get; set; }
        public MachineSpec machineSpec { get; set; }
        public int memoryLimit { get; set; }
        public int port { get; set; }
        public string regionID { get; set; }
        public string regionName { get; set; }
        public string status { get; set; }
    }

    private class ApiResponse
    {
        public List<Machines> machines { get; set; }
    }



    private async Task<(string address, int port)> RequestServerAllocation()
    {
        string url = $"{MultiplayApiBaseUrl}/projects/{projectId}/environments/{environmentId}/servers";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            string keySecret = "QmYivzvBXjs2xtkwDRXeJTAwXHqqJiBX";
            byte[] keyByteArray = Encoding.UTF8.GetBytes(apiKey + ":" + keySecret);
            string keybase64 = Convert.ToBase64String(keyByteArray);

            request.SetRequestHeader("Authorization", $"Basic {keybase64}");
            request.SetRequestHeader("Content-Type", "application/json");

            try
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.downloadHandler.text);
                    var response = JsonMapper.ToObject<List<Machines>>(request.downloadHandler.text);
                    var availableServer = response.Find(server => server.status == "AVAILABLE");

                    if (availableServer != null)
                    {
                        return (availableServer.ip, availableServer.port);
                    }
                    throw new System.Exception("No available servers found");
                }
                else
                {
                    throw new System.Exception($"Failed to get servers: {request.error}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error getting servers: {e.Message}");
                // Fallback to local testing values
                return ("127.0.0.1", 7777);
            }
        }
    }


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
        InitAuth();
        NetworkManager.Singleton.OnClientConnectedCallback += LoadGameScene;
        isLobbyHost = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution(Screen.width, Screen.height, true);
    }

   

    internal async void removeclientFromLobby()
    {
        await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
    }

    private void FixedUpdate()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && !UIManager.instance.Blocker.activeSelf)
        {
            UIManager.instance?.SetMessage("No Internet Connection. Please check your connection and restart the game");
        }
    }

    internal async void CloseLobby(string SetAI = "0")
    {
        JoinedLobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
        {
            IsLocked = true
        });
    }

    private void ClientStarted()
    {
        if (waitCor != null) StopCoroutine(waitCor);
        Debug.LogError("Client Started");
        LoadNetworkServerRpc(PlayerPrefs.GetString("Level"));
    }

    internal void StopCoroutineofWait()
    {
        if (waitCor != null) StopCoroutine(waitCor);
    }

    async void InitAuth()
    {
        UIManager.instance?.SetMessage("Connecting...");
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            string profile = "GummyBaby" + Random.Range(0, 100000).ToString();
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(profile);
            Application.targetFrameRate = 30;
            await UnityServices.InitializeAsync();
            if (AuthenticationService.Instance.IsSignedIn)
            {
                UIManager.instance.CloseMessage();
                return;
            }
            AuthenticationService.Instance.SwitchProfile(profile);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.LogError(UnityServices.State);

            if (AuthenticationService.Instance.IsSignedIn)
                UIManager.instance.CloseMessage();
            else
                UIManager.instance?.SetMessage("Couldn't sign in, Please check internet connection and try again");
        }
    }

    public async void CreatePublicLobby()
    {
        int n = int.Parse(SelectionManager.Instance.GetColor());
        try
        {
            UIManager.instance.Blocker.SetActive(true);
            UIManager.instance.LobbyName.text = "Connecting...";

            // Get server allocation
            var (serverAddress, serverPort) = await RequestServerAllocation();
            Debug.LogError(serverAddress + "----" + serverPort);

            JoinedLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby_" + CustomProperties.Instance.MyCode, MaxPlayers, new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {KEY_SERVER_ADDRESS, new DataObject(DataObject.VisibilityOptions.Member, serverAddress)},
                    {KEY_SERVER_PORT, new DataObject(DataObject.VisibilityOptions.Member, serverPort.ToString())},
                    {KEY_LEVEL_CODE, new DataObject(DataObject.VisibilityOptions.Public, PlayerPrefs.GetString("Level"))},
                    {KEY_REDCOLOR_CODE, new DataObject(DataObject.VisibilityOptions.Public, n.ToString())},
                    {KEY_BLUECOLOR_CODE, new DataObject(DataObject.VisibilityOptions.Public, SelectionManager.Instance.GetColor(n))},
                    {KEY_SKYBOX_Code, new DataObject(DataObject.VisibilityOptions.Public, SelectionManager.Instance.GetSky().ToString())},
                    {KEY_AI_NAME, new DataObject(DataObject.VisibilityOptions.Public, "Guest_" + GenerateRandomNumberString(8))},
                    {KEYSTORM, new DataObject(DataObject.VisibilityOptions.Public, Random.Range(0,2).ToString())},
                }
            });

            isLobbyHost = true;
            ConnectToServer(serverAddress, serverPort);
            waitCor = StartCoroutine(StartWaitTimer());
            CustomProperties.Instance.isRed = !(JoinedLobby.Players.Count % 2 == 0);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            UIManager.instance.Blocker.SetActive(false);
        }
    }

    private void ConnectToServer(string address, int port)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            address,
            (ushort)port
        );
        NetworkManager.Singleton.StartClient();
    }

    private IEnumerator StartWaitTimer()
    {
        int Timer = 5;

        START:
        UIManager.instance.LobbyName.text = "Waiting for Game to start...\n" + Timer.ToString();
        yield return new WaitForSeconds(1f);
        Timer--;
        if (Timer > 0)
            goto START;
        else
        {
            SetAIGame();
        }
    }

    void SetAIGame()
    {
        CloseLobby("1");
        Debug.LogError("Load scene");
        LoadNetworkServerRpc(PlayerPrefs.GetString("Level"));
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoadNetworkServerRpc(string targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }
    internal async void Getout()
    {
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            if (JoinedLobby != null && !IsServer) await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
            NetworkManager.Singleton.Shutdown();
        }
        GameSceneHasLoaded = false;
        Loader.Load(1);
    }

    private void LoadGameScene(ulong obj)
    {
        try
        {
            if (!GameSceneHasLoaded && NetworkManager.Singleton.IsServer && JoinedLobby.Players.Count > MinimumPlayerToStartGame)
            {
                GameSceneHasLoaded = true;
                if (waitCor != null) StopCoroutine(waitCor);
                LoadNetworkServerRpc(PlayerPrefs.GetString("Level"));
            }
        }
        catch
        {

        }
    }

    public async void JoinLobby()
    {
        try
        {
            UIManager.instance.Blocker.SetActive(true);
            UIManager.instance.LobbyName.text = "Connecting...";
            Debug.LogError("getting Lobies");
            List<Lobby> lobbies = (await LobbyService.Instance.QueryLobbiesAsync()).Results;
            Debug.LogError("Got Lobies "+lobbies.Count); 
            foreach (Lobby lobby2 in lobbies)
            {
                if (lobby2.Players.Count < lobby2.MaxPlayers && !lobby2.IsPrivate && lobby2.Data[KEY_LEVEL_CODE].Value == PlayerPrefs.GetString("Level"))
                {
                    Debug.LogError(lobby2.Name);
                    JoinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby2.Id);

                    string serverAddress = JoinedLobby.Data[KEY_SERVER_ADDRESS].Value;
                    int serverPort = int.Parse(JoinedLobby.Data[KEY_SERVER_PORT].Value);

                    ConnectToServer(serverAddress, serverPort);
                    UIManager.instance.LobbyName.text = "Waiting for Game to start...";
                    Debug.LogError(JoinedLobby.Players.Count);
                    CustomProperties.Instance.isRed = !(JoinedLobby.Players.Count % 2 == 0);
                    waitCor = StartCoroutine(StartWaitTimer());
                    return;
                }
            }
            CreatePublicLobby();
        }
        catch (LobbyServiceException e)
        {
            UIManager.instance.Blocker.SetActive(false);
        }
    }

    internal int getSkinColor(bool isRed)
    {
        return isRed ? int.Parse(JoinedLobby.Data[KEY_REDCOLOR_CODE].Value) : int.Parse(JoinedLobby.Data[KEY_BLUECOLOR_CODE].Value);
    }

    internal int GetSkybox()
    {
        return int.Parse(JoinedLobby.Data[KEY_SKYBOX_Code].Value);
    }

    internal int GetStorm()
    {
        return int.Parse(JoinedLobby.Data[KEYSTORM].Value);
    }

    string GenerateRandomNumberString(int length)
    {
        string result = "";
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            result += random.Next(0, 10).ToString();
        }
        return result;
    }
}

