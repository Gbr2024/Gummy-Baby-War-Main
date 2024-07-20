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
    string AIname;

    private void Update()
    {
        if (IsOwner && IsServer)
        {
            if(targetpos != Vector3.zero) transform.LookAt(targetpos);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position,targetpos)<10f && !bombset)
            {
                bombset = true;
                targetpos = Vector3.zero;
                SetBombServerRpc();
                
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

    int i = 0;

    [ServerRpc]
    void SetBombServerRpc()
    {
        Debug.LogError(i);
        if (!IsServer || i>0) return;
        i++;

        if(isTargetAI)
        {
            foreach (var item in FindObjectsOfType<PlayerController>())
            {
                if (item.AIname == AIname)
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

        
        try
        {
            var drop = NetworkManager.Instantiate(Bomb);
            drop.id = PlayerID;
            drop.isRed = isRed;

            drop.NetworkObject.Spawn();
            Debug.LogError("spawned");
        }
        catch(Exception e)
        {
            Debug.LogError("something went wrong");
            Debug.LogError(e.Message);
        }
        

    }

    [ClientRpc]
    internal void SetTargetClientRpc(ulong ownerClientId, ulong id, bool isred,bool isAI=false,string Aname="")
    {
        isRed = isred;
        TargetId = id;
        PlayerID = ownerClientId;
        isTargetAI = isAI;
        AIname = Aname;
    }
}
