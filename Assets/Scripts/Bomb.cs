using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bomb : NetworkBehaviour
{
    [SerializeField] Impact impact;
    internal ulong id;
    internal bool isRed;

    bool isSet = false;
    private void Blast()
    {
        if (isSet) return;

        isSet = true;
        BlastServerRpc(id, isRed);
    }

    [ServerRpc(RequireOwnership = false)]
    private void BlastServerRpc(ulong id, bool isRed)
    {
        impact.transform.position = transform.position;
        var effect = NetworkManager.Instantiate(impact).GetComponent<Impact>();
        effect.PlayerID.Value = id;
        effect.isRed.Value = isRed;
        effect.NetworkObject.SpawnWithOwnership(OwnerClientId, true);
        NetworkObject.Despawn(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsOwner && !isSet)
            Blast();
    }
}
