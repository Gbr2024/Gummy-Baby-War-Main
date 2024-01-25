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

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;


    //Lobby Settings
    public TMP_Text LobbyName;
    const int MaxPlayers = 2;
    private Lobby JoinedLobby;
    internal bool GameSceneHasLoaded = false;

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
        InitAuth();
        NetworkManager.Singleton.OnClientConnectedCallback += LoadGameScene;
        NetworkManager.Singleton.OnClientStarted += ClientStarted;
    }

    private void ClientStarted()
    {
        Debug.LogError("Client Started");
    }

    async void InitAuth()
    {
        if(UnityServices.State!=ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(Random.Range(0, 100000).ToString());
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.LogError(UnityServices.State);
        }
    }



    public async void CreatePublicLobby()
    {
        try
        {
            JoinedLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby_"+CustomProperties.Instance.MyCode, MaxPlayers, new CreateLobbyOptions
            {
                IsPrivate = false,
                
            });
            Debug.LogError(JoinedLobby.Name);
            LobbyName.text = JoinedLobby.Name;
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = GetLocalIPAddress();
            NetworkManager.Singleton.StartHost();
            CustomProperties.Instance.isRed = !(JoinedLobby.Players.Count % 2 == 0);
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private void LoadGameScene(ulong obj)
    {
        Debug.LogError("Joined");
       if(!GameSceneHasLoaded && NetworkManager.Singleton.IsServer && JoinedLobby.Players.Count>2)
        {
            GameSceneHasLoaded = true;
            Loader.LoadNetwork(Loader.Gamescenes[Random.Range(0, Loader.Gamescenes.Length)]);
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
            List<Lobby> lobbies = (await LobbyService.Instance.QueryLobbiesAsync()).Results;
            foreach (Lobby lobby2 in lobbies)
            {
                if(lobby2.Players.Count<lobby2.MaxPlayers)
                {
                    Debug.LogError(lobby2.Name);
                    JoinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby2.Id);
                    LobbyName.text = JoinedLobby.Name;
                    NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = GetLocalIPAddress();
                    NetworkManager.Singleton.StartClient();
                    Debug.LogError(JoinedLobby.Players.Count);
                    CustomProperties.Instance.isRed = !(JoinedLobby.Players.Count % 2 == 0);
                }
            }
            
           
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
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
}
