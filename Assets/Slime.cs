using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Slime : NetworkBehaviour
{
    public float Damage = 100f;
    internal ulong id;
    internal bool isRed;

    internal bool isActive = true;
    private void Start()
    {
        if(IsServer)
        {
            Invoke(nameof(DespawninTime), 10f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
            isActive = false;
    }

    void DespawninTime()
    {
        NetworkObject.Despawn(true);
    }
}
