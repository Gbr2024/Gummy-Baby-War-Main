using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Slime : NetworkBehaviour
{
    [SerializeField] Impact impact;
    public float Damage = 100f;
    internal ulong id;
    internal bool isRed;
    Rigidbody rb;

    internal bool isActive = true;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if(IsServer)
        {
            Invoke(nameof(DespawninTime), 10f);
        }
        if(IsServer)
            rb.AddForce(-Vector3.up * 30, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isActive)
        {
            isActive = false;
            BlastServerRpc();
        }
       
    }

    [ServerRpc(RequireOwnership = false)]
    private void BlastServerRpc()
    {
        impact.transform.position = transform.position;
        var effect = NetworkManager.Instantiate(impact).GetComponent<Impact>();
        effect.PlayerID.Value = id;
        effect.isRed.Value = isRed;
        effect.NetworkObject.SpawnWithOwnership(OwnerClientId, true);
        NetworkObject.Despawn(true);
    }

    void DespawninTime()
    {
        NetworkObject.Despawn(true);
    }
}
