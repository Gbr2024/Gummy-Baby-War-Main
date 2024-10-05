using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System;

public class Impact : NetworkBehaviour
{
    [SerializeField] SphereCollider Scollider;
    [SerializeField] float Damage,RadiusToInc=7.5f;

    internal float DamagetoApply;

    public NetworkVariable<bool> isRed = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<ulong> PlayerID = new NetworkVariable<ulong>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //internal ulong PlayerID;
    //internal bool isRed;
    Vector3 startPos;


    // Start is called before the first frame update
    
    void Start()
    {
        DOTween.To(() => Scollider.radius, x => Scollider.radius = x, RadiusToInc, 0.7f)
               .OnUpdate(() => DamagetoApply = Damage / Scollider.radius).OnComplete(() => { Scollider.enabled = false; }).SetDelay(.25f);
        Invoke(nameof(Destroyimpact), 3f);
    }

    private void Destroyimpact()
    {
        DisableGrenadeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisableGrenadeServerRpc()
    {
        NetworkObject.Despawn(true);
    }
}


