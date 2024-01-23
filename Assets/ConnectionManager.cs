using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class ConnectionManager : NetworkBehaviour
{
    // Start is called before the first frame update
    // Flag to check if the player is trying to join a game
    [SerializeField] int MaxConnections = 10;
    [SerializeField] List<ulong> Players = new();
    [SerializeField] List<string> PlayersName = new();
    [SerializeField] NetworkObject networkObject;
    private bool isJoiningGame = false;
    [SerializeField] int[] GameBuildIndexes;
    private void Start()
    {
        if(!PlayerPrefs.HasKey("PlayerName"))
            PlayerPrefs.SetString("PlayerName", "Player" + Random.Range(11111, 99999));

        Debug.LogError(PlayerPrefs.GetString("PlayerName"));
        //NetworkManager. += ClientConnectionFail;
        //NetworkManager.Singleton.on

        NetworkManager.OnClientStarted += OnClientConnectedClientRpc;
        NetworkManager.OnClientConnectedCallback += ClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientDisconnected;
    }



  
    public void JoinGame()
    {
        if (IsServer)
        {
            Debug.LogError("Cannot join game on the server.");
            return;
        }

        // Connect to the specified server
        NetworkManager.Singleton.StartClient();
        isJoiningGame = true;
       
    }

    private void ClientDisconnected(ulong obj)
    {
        Debug.LogError(NetworkManager.DisconnectReason);
    }

    [ClientRpc]
    private void CustomDisconnectMessageClientRpc(ulong targetClientId, string disconnectReason)
    {
        // This method is called on the client before it gets disconnected
        Debug.Log($"Received custom disconnect message for client {targetClientId}: {disconnectReason}");
    }




    // Method to create a new game
    public void CreateGame()
    {
        // Check if already a server
        if (IsServer)
        {
            Debug.LogError("Already running as a server.");
            return;
        }

        // Check if already a client
        if (IsClient)
        {
            Debug.LogError("Cannot create a new game while already connected as a client.");
            return;
        }

        // Start the server
        NetworkManager.Singleton.StartHost();

        isJoiningGame = true;
    }

    private void ClientConnected(ulong ClientId)
    {
        Debug.LogError("Client Connected "+ ClientId);
        Debug.LogError(IsServer);
        Debug.LogError(IsLocalPlayer);
        if(IsServer)
        {
            setPLayerNameServerRpc(CustomProperties.Instance.MyCode, networkObject.OwnerClientId);
            if (NetworkManager.Singleton.ConnectedClients.Count > MaxConnections)
            {
                NetworkManager.Singleton.DisconnectClient(ClientId, "Maximum Players reached");
                return;
            }
        }
        else
        {
            LoadGameSceneServerRpc();
            //setPLayerNameServerRpc(CustomProperties.Instance.MyCode, networkObject.OwnerClientId);
        }
    }

    

    // Callback method for when the client successfully connects to the server
    [ClientRpc]
    private void OnClientConnectedClientRpc()
    {
        CustomProperties.Instance.isRed = true;
        
        //Debug.LogError("Been called");
        //if (isJoiningGame)
        //{
        //    // Load the game scene after successfully connecting to the server
        //    SceneManager.LoadScene(GameBuildIndexes[Random.Range(0, GameBuildIndexes.Length)]);
        //}
    }

    // Callback method for when the server is started
   

    
    
    [ServerRpc (RequireOwnership = false)]
    public void setPLayerNameServerRpc(string Code,ulong ClientID)
    {
        PlayersName.Add(Code);
        Players.Add(ClientID);
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void LoadGameSceneServerRpc()
    {
        LoadGameSceneAllClientRpc();
    }

    [ClientRpc]
    public void LoadGameSceneAllClientRpc()
    {
        SceneManager.LoadScene(GameBuildIndexes[Random.Range(0, GameBuildIndexes.Length)]);
    }

    public void TestLoad()
    {
        SceneManager.LoadScene(GameBuildIndexes[Random.Range(0, GameBuildIndexes.Length)]);
    }
}
