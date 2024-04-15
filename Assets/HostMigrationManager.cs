using System.Linq;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Networking;

public class HostMigrationManager : NetworkBehaviour
{
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
        networkManager.OnClientDisconnectCallback += OnServerDisconnect;
    }

    private void OnServerDisconnect(ulong conn)
    {
        if (conn == NetworkManager.ServerClientId && !ScoreManager.Instance.GameHasFinished) // If the host disconnected
        {
            // Select a new host (you might implement your own logic here)
            //var newHost = GetNewHost(conn);
            setNewHostForLobby();

        }
    }

    public void setNewHostForLobby()
    {
       
    }

    // Helper function to select a new host from remaining players
    private NetworkClient GetNewHost(ulong id)
    {
        foreach (var player in networkManager.ConnectedClients)
        {
            if (player.Key != id)
            {
                return player.Value;
            }
        }
        return null;
    }

}