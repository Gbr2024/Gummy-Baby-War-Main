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
    bool isTargetAI = false;
    bool fire=false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }



    [ClientRpc]
    internal void SetTargetClientRpc(ulong id, ulong playerID, bool isRed, bool TargetAI = false)
    {
        if (!isTargetAI)
        {
            foreach (var item in FindObjectsOfType<WBThirdPersonController>())
            {
                if (item.OwnerClientId == id)
                {
                    gun.setLookTarget(item.transform);
                    targethealth = item.GetComponent<HealthManager>();
                }
            }
        }
        else
        {
            foreach (var item in FindObjectsOfType<PlayerController>())
            {
                if (item.OwnerClientId == id)
                {
                    gun.setLookTarget(item.transform);
                    targetAIhealth = item.GetComponent<AIHealth>();
                }
            }
        }
        Teamid = playerID;
        TeamisRed = isRed;
        fire = true;
    }

    HealthManager targethealth;
    AIHealth targetAIhealth;
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
        if(fire && gun._currentAmmo>0 && ((!isTargetAI && !targethealth.isDead) || (isTargetAI && !targetAIhealth.isDead)))
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
