using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PathCreation.Examples;
using WeirdBrothers.ThirdPersonController;

public class FlyController : NetworkBehaviour
{
    [SerializeField] PathFollower pathFollower;

    [SerializeField] GunController gun;
    bool TeamisRed;
    ulong Teamid;

    bool fire=false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShootServerRpc()
    {
        ShootNAllClientRpc();
    }


    [ClientRpc]
    internal void ShootNAllClientRpc()
    {
        fire = true;
        //
    }



    [ClientRpc]
    internal void SetTargetClientRpc(ulong id,ulong playerID,bool isRed)
    {
        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            if (item.OwnerClientId == id)
            {
                gun.setLookTarget(item.transform);
                targethealth = item.GetComponent<HealthManager>();
            }
        }
        Teamid = playerID;
        TeamisRed = isRed;
    }

    HealthManager targethealth;
    bool isdone = false;

    private void FixedUpdate()
    {
        if(IsServer)
        {
            if(gun._currentAmmo<=0)
            {
                if (isdone ) return;
                isdone = true;
                Destroy(pathFollower.pathCreator.gameObject);
                NetworkObject.Despawn(true);
            }
            if (targethealth.isDead)
            {
                if (isdone) return;
                isdone = true;
                Destroy(pathFollower.pathCreator.gameObject);
                NetworkObject.Despawn(true);
                fire = false;

            }
        }
        if(fire && gun._currentAmmo>0 && !targethealth.isDead)
        {
            gun.FireBullet(TeamisRed, Teamid);
        }
    }

    

    internal void SetData(ulong id, bool isRed)
    {
        Teamid = id;
        TeamisRed = isRed;
    }

    internal void SetPath(PathCreation.PathCreator path)
    {
        pathFollower.pathCreator = path;
    }
}
