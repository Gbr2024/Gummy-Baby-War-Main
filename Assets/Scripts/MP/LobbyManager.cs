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
using System.Threading.Tasks;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;


    //Lobby Settings
    const int MaxPlayers = 10,MinimumPlayerToStartGame=2;
    private readonly string KEY_START_CODE="JoinCode";
    private readonly string KEY_REDCOLOR_CODE="RedColor";
    private readonly string KEY_BLUECOLOR_CODE="BlueColor";
    private readonly string KEY_SKYBOX_Code="Skybox";
    private readonly string KEY_LEVEL_CODE="Level";
    private readonly string KEY_AI_NAME="AIname";
    private readonly string KEYSTORM="EnableStorm";

    internal Lobby JoinedLobby;
    internal bool GameSceneHasLoaded = false;
    bool isLobbyHost = false;
    internal bool GameHasStarted = false;
    private string oldHost;
    private Coroutine waitCor;

    public string OldRelayJoinCode { get; private set; }

    
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

        //NetworkManager.Singleton.OnClientStarted += ClientStarted;
        isLobbyHost = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Test(ulong obj)
    {
        Debug.LogError("Here");
    }

    internal async void removeclientFromLobby()
    {
        await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, JoinedLobby.HostId);
    }

    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && !UIManager.instance.Blocker.activeSelf)
        {
            UIManager.instance?.SetMessage("No Internet Connetction. Please check you connection and restart the game");
        }

        //if (JoinedLobby != null)//when host leaves, lobby service automaticaly pick new host and updates hois host id
        //{
        //    if (oldHost != JoinedLobby.HostId) // so if my cahed "oldhost" is not equol to new host id
        //    {
        //        if (JoinedLobby.HostId == AuthenticationService.Instance.PlayerId) //if im the host i start new relay
        //        {
        //            //  NetworkManager.Singleton.Shutdown();
        //            CreateNewRelayOnHostDc();
        //            oldHost = JoinedLobby.HostId;
        //            Debug.Log("starting new relay");
        //        }
        //        else
        //        {

        //            if (JoinedLobby.Data.ContainsKey(KEY_START_CODE)) // if im client (also i didnt checked if this part works with more than 2 players)
        //            {
        //                if (OldRelayJoinCode != JoinedLobby.Data[KEY_START_CODE].Value) // check if cached relay is diferent than new relay
        //                {
        //                    Debug.Log("joining new relay");
        //                    JoinNewRelay(); // join new relay
        //                }
        //            }
        //        }
        //    }
        //}
    }

   

    private async void CreateNewRelayOnHostDc()
    {
        await RelayManager.Instance.CreateRelay(MaxPlayers);
        JoinedLobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
                {
                    {KEY_START_CODE,new DataObject(DataObject.VisibilityOptions.Member,RelayManager.Instance.joincode) }
                }
        });
        NetworkManager.Singleton.StartHost();
    }

    internal async void CloseLobby(string SetAI="0")
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
        Loader.LoadNetwork(PlayerPrefs.GetString("Level"));
    }

    internal void StopCoroutineofWait()
    {
        if (waitCor != null) StopCoroutine(waitCor);
    }

    async void InitAuth()
    {
        UIManager.instance?.SetMessage("Connecting...");
        if(UnityServices.State!=ServicesInitializationState.Initialized)
        {
            string profile = "GummyBaby" + Random.Range(0, 100000).ToString();
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(profile);
            await UnityServices.InitializeAsync();
            if (AuthenticationService.Instance.IsSignedIn)
            {
                UIManager.instance.CloseMessage();
                return;
            }
            AuthenticationService.Instance.SwitchProfile(profile);
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
                    {KEY_BLUECOLOR_CODE,new DataObject(DataObject.VisibilityOptions.Public,SelectionManager.Instance.GetColor(n)) },
                    {KEY_SKYBOX_Code,new DataObject(DataObject.VisibilityOptions.Public,SelectionManager.Instance.GetSky().ToString()) },
                    {KEY_AI_NAME,new DataObject(DataObject.VisibilityOptions.Public,"Guest_"+GenerateRandomNumberString(8)) },
                    {KEYSTORM,new DataObject(DataObject.VisibilityOptions.Public,Random.Range(0,2).ToString() )},
                }
            });
            await RelayManager.Instance.CreateRelay(MaxPlayers);
            isLobbyHost = true;
            oldHost = JoinedLobby.HostId;
            JoinedLobby = await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data=new Dictionary<string, DataObject>
                {
                    {KEY_START_CODE,new DataObject(DataObject.VisibilityOptions.Member,RelayManager.Instance.joincode) }
                }
            });
            OldRelayJoinCode = RelayManager.Instance.joincode;
            Debug.LogError(RelayManager.Instance.joincode);
            //Debug.LogError(NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address);
            //NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = GetLocalIPAddress();
            NetworkManager.Singleton.StartHost();
            waitCor= StartCoroutine(StartWaitTimer());
            CustomProperties.Instance.isRed = !(JoinedLobby.Players.Count % 2 == 0);
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
            UIManager.instance.Blocker.SetActive(false);
        }
    }

    private IEnumerator StartWaitTimer()
    {
        int Timre = 5;

        START:
        UIManager.instance.LobbyName.text = "Waiting for Game to start...\n" + Timre.ToString() ;
        yield return new WaitForSeconds(1f);
        Timre--;
        if (Timre > 0)
            goto START;
        else
        {
            SetAIGame();
        }
    }

    void SetAIGame()
    {
        CloseLobby("1");
        Loader.LoadNetwork(PlayerPrefs.GetString("Level"));
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
                if (waitCor != null) StopCoroutine(waitCor);
                Loader.LoadNetwork(PlayerPrefs.GetString("Level"));
                AdmobAds.Instance.DestroyBannerAd();
            }
        }
        catch
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
                    oldHost = JoinedLobby.HostId;
                    OldRelayJoinCode = JoinedLobby.Data[KEY_START_CODE].Value;
                    if (JoinedLobby.Data[KEY_START_CODE].Value!="0")
                    {
                        if(!isLobbyHost)
                        {
                            await RelayManager.Instance.JoinRelay(JoinedLobby.Data[KEY_START_CODE].Value);
                        }
                    }
                    
                    //NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = GetLocalIPAddress();
                    NetworkManager.Singleton.StartClient();
                    UIManager.instance.LobbyName.text = "Waiting for Game to start...";
                    Debug.LogError(JoinedLobby.Players.Count);
                    CustomProperties.Instance.isRed = !(JoinedLobby.Players.Count % 2 == 0);
                    waitCor=StartCoroutine(StartWaitTimer());
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
            result += random.Next(0, 10).ToString(); // Generating random digits (0-9)
        }

        return result;
    }
}
