using UnityEngine;
using Unity.Netcode;
using WeirdBrothers.ThirdPersonController;
using DG.Tweening;

public class Crate : NetworkBehaviour
{
    public int KillStreak;
    GameObject particleEffect;
    // Start is called before the first frame update

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        particleEffect = transform.GetChild(0).gameObject;
        Destroy(particleEffect, 12f);
        if (IsServer)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            KillStreak = Random.Range(2, 6);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(IsServer)
        {
            if(collision.transform.TryGetComponent(out WBThirdPersonController controller))
            {
                Destroy(particleEffect);
                controller.SetKillStreakClientRPC(KillStreak,controller.OwnerClientId);
                transform.DOScale(Vector3.zero, .5f).OnComplete(()=> { DestrouCrateServerRpc(); });
            }
        }
    }

    [ServerRpc]
    void DestrouCrateServerRpc()
    {
        NetworkObject.Despawn(true);
    }
}
