using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using WeirdBrothers.ThirdPersonController;

public class Grenade : NetworkBehaviour
{
    [SerializeField] GameObject Effect;
    public Rigidbody rb;
    public Transform ToFollow;
    internal ulong PlayerID;
    internal bool hasCollided = false;
    internal bool hasThrown = false;
    internal bool isRed;

    // Start is called before the first frame update
    void Start()
    {
        rb.isKinematic = true;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            transform.rotation = Quaternion.identity;
            CancelInvoke(nameof(Blast));
            Invoke(nameof(Blast), 7f);

            foreach (var item in FindObjectsOfType<WBThirdPersonController>())
            {
                if (item.OwnerClientId == OwnerClientId)
                {
                    item.Context.trajectory.grenade = this;
                    ToFollow = item.Context.GrenadeHandPos;
                }
            }
        }
    }

    bool isSet = false;
    private void Blast()
    {
        if (isSet) return;

        isSet = true;
        BlastServerRpc(NetworkObject.OwnerClientId,isRed);
    }

    [ServerRpc (RequireOwnership =false)]
    private void BlastServerRpc(ulong id,bool isRed)
    {

        Effect.transform.position = transform.position;
        var effect = NetworkManager.Instantiate(Effect).GetComponent<Impact>() ;
        
        effect.PlayerID=id;
        effect.isRed=isRed;
        effect.NetworkObject.SpawnWithOwnership(OwnerClientId, true);
        NetworkObject.Despawn(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(IsOwner && !rb.isKinematic)
            hasCollided = true;
    }

    private void Update()
    {
        if (IsOwner && ToFollow != null)
        {
            transform.position = ToFollow.position;
            transform.forward = ToFollow.forward;
        }
    }


}
