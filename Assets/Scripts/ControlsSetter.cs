using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ControlsSetter : NetworkBehaviour
{
    private void Awake()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        transform.position = PlayerSetManager.instance.RedCribs[Random.Range(0, 3)].position;
        if (!IsOwner)
            enabled = false;
    }

   



    private void Start()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            PlayerSetManager.instance.setPlayerControlandCam(gameObject);
        }
    }

    



    //[ServerRpc]
    //public void SetPosServerRpc(string code,bool isRed)
    //{
        
    //}
}
