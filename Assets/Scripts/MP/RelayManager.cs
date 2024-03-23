using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;


    internal Allocation allocation;
    internal JoinAllocation joinallocation;
    internal string joincode;

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
    internal async Task CreateRelay(int Maxplayer)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(Maxplayer);
            joincode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    internal async Task JoinRelay(string joincode)
    {
        try
        {
            joinallocation= await RelayService.Instance.JoinAllocationAsync(joincode);
            RelayServerData relayServerData = new RelayServerData(joinallocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    
}
