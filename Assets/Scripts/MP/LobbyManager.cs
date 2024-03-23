using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using System.Net;
using Unity.Netcode.Transports.UTP;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;


    //Lobby Settings
    const int MaxPlayers = 10,MinimumPlayerToStartGame=2;
    private readonly string KEY_START_CODE="JoinCode";
    private readonly string KEY_REDCOLOR_CODE="RedColor";
    private readonly string KEY_BLUECOLOR_CODE="BlueColor";
    private readonly string KEY_LEVEL_CODE="Level";
    private Lobby JoinedLobby;
    internal bool GameSceneHasLoaded = false;
    bool isLobbyHost = false;
    internal bool GameHasStarted = false;

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
        NetworkManager.Singleton.OnClientStarted += ClientStarted;
        isLobbyHost = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && !UIManager.instance.Blocker.activeSelf)
        {
            UIManager.instance?.SetMessage("No Internet Connetction. Please check you connection and restart the game");
        }
    }

    internal async void CloseLobby()
    {
        JoinedLobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
        {
            IsLocked = true
        });
    }

    private void ClientStarted()
    {
        Debug.LogError("Client Started");
        Loader.LoadNetwork(Loader.Gamescenes[Random.Range(0, Loader.Gamescenes.Length)]);
    }

    async void InitAuth()
    {
        UIManager.instance?.SetMessage("Connecting...");
        if(UnityServices.State!=ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(Random.Range(0, 100000).ToString());
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.LogError(UnityServices.State);
            if(AuthenticationService.Instance.IsSignedIn)
                UIManager.instance.CloseMessage();
            else
                UIManager.instance?.SetMessage("Couldn't sign in, Please check internet connection and try again");
        }
    }



    public async void CreatePublicLobby()
    {
        int n =0;
        n =int.Parse(SelectionManager.Instance.GetColor());
        try
        {
            UIManager.instance.Blocker.SetActive(true);
            UIManager.instance.LobbyName.text = "Connecting...";
            JoinedLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby_"+CustomProperties.Instance.MyCode, MaxPlayers, new CreateLobbyOptions
            {
                IsPrivate = false,
                Data=new Dictionary<string, DataObject>
                {
                    {KEY_START_CODE,new DataObject(DataObject.VisibilityOptions.Member,"0") },
                    {KEY_LEVEL_CODE,new DataObject(DataObject.VisibilityOptions.Public,PlayerPrefs.GetString("Level")) },
                    {KEY_REDCOLOR_CODE,new DataObject(DataObject.VisibilityOptions.Public,n.ToString()) },
                    {KEY_BLUECOLOR_CODE,new DataObject(DataObject.VisibilityOptions.Public,SelectionManager.Instance.GetColor(n)) }
                }
            });
            await RelayManager.Instance.CreateRelay(MaxPlayers);
            isLobbyHost = true;
            JoinedLobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data=new Dictionary<string, DataObject>
                {
                    {KEY_START_CODE,new DataObject(DataObject.VisibilityOptions.Member,RelayManager.Instance.joincode) }
                }
            });
            
            Debug.LogError(RelayManager.Instance.joincode);
            //Debug.LogError(NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address);
            //NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = GetLocalIPAddress();
            NetworkManager.Singleton.StartHost();
            CustomProperties.Instance.isRed = !(JoinedLobby.Players.Count % 2 == 0);
            UIManager.instance.LobbyName.text = "Waiting for Game Start...";
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
            UIManager.instance.Blocker.SetActive(false);
        }
    }

    internal async void Getout()
    {
        if(NetworkManager.Singleton.IsConnectedClient)
        {
            if (JoinedLobby != null && IsServer) await LobbyService.Instance.DeleteLobbyAsync(JoinedLobby.Id);
            if (JoinedLobby != null && !IsServer) await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
            Debug.LogError("HERE");
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
                Loader.LoadNetwork(PlayerPrefs.GetString("Level"));
                AdmobAds.Instance.DestroyBannerAd();
            }
        }
        catch(Exception e)
        {
            
        }
       
    }

    private void NewPlayerJoined(List<LobbyPlayerJoined> obj)
    {
        Debug.LogError("Player Joined");
        foreach (var item in obj)
        {
            Debug.LogError(item.PlayerIndex);
        }
    }

    public async void JoinLobby()
    {
        try
        {
            UIManager.instance.Blocker.SetActive(true);
            UIManager.instance.LobbyName.text = "Connecting...";
            List<Lobby> lobbies = (await LobbyService.Instance.QueryLobbiesAsync()).Results;
            foreach (Lobby lobby2 in lobbies)
            {
                if(lobby2.Players.Count<lobby2.MaxPlayers && !lobby2.IsPrivate && lobby2.Data[KEY_LEVEL_CODE].Value== PlayerPrefs.GetString("Level"))
                {
                    Debug.LogError(lobby2.Name);
                    JoinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby2.Id);
                    if(JoinedLobby.Data[KEY_START_CODE].Value!="0")
                    {
                        if(!isLobbyHost)
                        {
                            await RelayManager.Instance.JoinRelay(JoinedLobby.Data[KEY_START_CODE].Value);
                        }
                    }
                    //NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = GetLocalIPAddress();
                    NetworkManager.Singleton.StartClient();
                    UIManager.instance.LobbyName.text = "Waiting for Game Start...";
                    Debug.LogError(JoinedLobby.Players.Count);
                    CustomProperties.Instance.isRed = !(JoinedLobby.Players.Count % 2 == 0);
                    return;
                }
            }
            CreatePublicLobby();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            UIManager.instance.Blocker.SetActive(false);
        }
    }

   

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                string cleanedIP = System.Text.RegularExpressions.Regex.Replace(ip.ToString(), "[^0-9.]", "");
                return cleanedIP;
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    internal int getSkinColor(bool isRed)
    {
        return isRed ? int.Parse(JoinedLobby.Data[KEY_REDCOLOR_CODE].Value) : int.Parse(JoinedLobby.Data[KEY_BLUECOLOR_CODE].Value);
        
    }
}
