using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlanerController : NetworkBehaviour
{
    [SerializeField] Bomb Bomb;
    Vector3 targetpos;
    ulong TargetId, PlayerID;
    bool isRed;
    bool bombset = false;
    public float speed=150f;
    bool isTargetAI;

    private void Update()
    {
        if (IsOwner && IsServer)
        {
            if(targetpos != Vector3.zero) transform.LookAt(targetpos);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position,targetpos)<10f && !bombset)
            {
                targetpos = Vector3.zero;
                SetBombServerRpc();
                bombset = true;
            }
            Invoke(nameof(Desp), 30f);
        }
    }

    private void Desp()
    {
        NetworkObject.Despawn(true);
    }

    internal void SetTarget(Vector3 pos,bool target=false)
    {
        pos = new Vector3(pos.x, transform.position.y, pos.z);
        targetpos = pos;
        isTargetAI = target;
    }

    [ServerRpc]
    void SetBombServerRpc()
    {
        if(isTargetAI)
        {
            foreach (var item in FindObjectsOfType<PlayerController>())
            {
                if (item.OwnerClientId == TargetId)
                    Bomb.transform.position = new Vector3(item.transform.position.x, 60f, item.transform.position.z);
            }
        }
        else
        {
            foreach (var item in FindObjectsOfType<WeirdBrothers.ThirdPersonController.WBThirdPersonController>())
            {
                if (item.OwnerClientId == TargetId)
                    Bomb.transform.position = new Vector3(item.transform.position.x, 60f, item.transform.position.z);
            }

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
