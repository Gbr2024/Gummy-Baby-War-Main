using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

// Place this script on the same GameObject as NetworkManager
public class NetworkValidator : MonoBehaviour
{
    private Dictionary<ulong, float> clientLastMessageTime = new Dictionary<ulong, float>();
    private Dictionary<ulong, int> clientMessageCount = new Dictionary<ulong, int>();
    private const float MESSAGE_RATE_LIMIT = 100; // messages per second
    private const float TIMEOUT_DURATION = 30f;

    private void Start()
    {
        if (!ServerManager.Instance.IsServer) return;

        NetworkManager.Singleton.ConnectionApprovalCallback += ApproveConnection;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void ApproveConnection(NetworkManager.ConnectionApprovalRequest request,
                                 NetworkManager.ConnectionApprovalResponse response)
    {
        // Add any connection validation logic here
        response.Approved = true;
        response.CreatePlayerObject = true;
    }

    private void OnClientConnected(ulong clientId)
    {
        clientLastMessageTime[clientId] = Time.time;
        clientMessageCount[clientId] = 0;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        clientLastMessageTime.Remove(clientId);
        clientMessageCount.Remove(clientId);
    }

    public bool ValidateMessage(ulong clientId)
    {
        if (!clientLastMessageTime.ContainsKey(clientId))
            return false;

        float currentTime = Time.time;
        float deltaTime = currentTime - clientLastMessageTime[clientId];

        // Reset message count every second
        if (deltaTime >= 1f)
        {
            clientMessageCount[clientId] = 0;
            clientLastMessageTime[clientId] = currentTime;
        }

        // Check rate limiting
        clientMessageCount[clientId]++;
        if (clientMessageCount[clientId] > MESSAGE_RATE_LIMIT)
        {
            ServerLogger.LogWarning($"Client {clientId} exceeded message rate limit");
            return false;
        }

        // Check timeout
        if (deltaTime > TIMEOUT_DURATION)
        {
            ServerLogger.LogWarning($"Client {clientId} timed out");
            NetworkManager.Singleton.DisconnectClient(clientId);
            return false;
        }

        return true;
    }
}