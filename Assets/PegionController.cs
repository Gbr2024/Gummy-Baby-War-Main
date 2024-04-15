using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PegionController : NetworkBehaviour
{
    [SerializeField] Slime Bomb;
    Transform targetpos;
    ulong TargetId, PlayerID;
    bool isRed;
    bool bombset = false;
    private float speed = 40f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsServer)
        {
            Invoke(nameof(Desp), 30f);
        }
    }

    private void Update()
    {
        if (IsOwner && IsServer)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            var pos = new Vector3(targetpos.position.x, 35f, targetpos.position.z);
            if (targetpos != null) transform.LookAt(pos);
            if (Vector3.Distance(transform.position, pos) < 5f && !bombset)
            {
                targetpos = null;
                SetBombServerRpc();
                
                bombset = true;
            }
            
        }
    }

    private void Desp()
    {
        NetworkObject.Despawn(true);
    }

    internal void SetTarget(Transform pos)
    {
        targetpos = pos;
    }

    [ServerRpc]
    void SetBombServerRpc()
    {
        foreach (var item in FindObjectsOfType<WeirdBrothers.ThirdPersonController.WBThirdPersonController>())
        {
            if (item.OwnerClientId == TargetId)
                Bomb.transform.position = new Vector3(item.transform.position.x, 35f, item.transform.position.z);
        }
        var drop = NetworkManager.Instantiate(Bomb);
        drop.id = PlayerID;
        drop.isRed = isRed;
        drop.NetworkObject.Spawn();
    }

    [ClientRpc]
    internal void SetTargetClientRpc(ulong ownerClientId, ulong id, bool isred)
    {
        isRed = isred;
        TargetId = id;
        PlayerID = ownerClientId;
    }
}
