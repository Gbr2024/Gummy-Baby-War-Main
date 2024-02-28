using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using WeirdBrothers.ThirdPersonController;
using Random = UnityEngine.Random;

public class HealthManager : NetworkBehaviour
{
    [SerializeField] float CurrentHealth;
    [SerializeField] RagdollController ragdollController;
    [SerializeField] WBThirdPersonController controller;
    float Health = 100;

    internal bool isDead = false;
    internal bool isActivated = false;

    private void Awake()
    {
        CurrentHealth = Health;
    }

    private void Start()
    {
        if(IsOwner)
        {
            isActivated = true;
            GetComponent<Syncer>().Activated.Value = true;
            WBUIActions.UpdateHealth?.Invoke(CurrentHealth / Health);
        }
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (isDead || !LobbyManager.Instance.GameHasStarted || !isActivated) return;
       
        if (collision.gameObject.TryGetComponent<Bullet>(out Bullet bullet))
        {
            //Debug.LogError("Damaging");
            if (NetworkManager.Singleton.LocalClientId != bullet.PlayerID) return;
            {
                if (CurrentHealth - bullet.damage <= 0) controller.SetKill();
                controller.AddDamage(bullet.damage, NetworkObject.OwnerClientId);
            }
        }
    }

    internal void AddDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0) CurrentHealth = 0;
        if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            
            WBUIActions.UpdateHealth?.Invoke(CurrentHealth / Health);
            if (CurrentHealth == 0)
            {
                PlayerSetManager.instance.ChangeView(false);
                Invoke(nameof(resetPlayer), 5f);
            }
        }

        if (CurrentHealth == 0)
        {
            ragdollController.SetToAll(true);
           
            isDead = true;
        }
    }

    void resetPlayer()
    {
        //ragdollController.SetToAll(false);
        //ResetHealth();
        //if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        //    transform.position = CustomProperties.Instance.isRed ? PlayerSetManager.instance.RedCribs[Random.Range(0, 3)].position : PlayerSetManager.instance.BlueCribs[Random.Range(0, 3)].position;
        DestroyPlayerServerRPC(NetworkObject.OwnerClientId);
        PlayerSetManager.instance.SpinTheWheel();
    }

    internal void ResetHealth()
    {
        isDead = false;
        CurrentHealth = Health;
        if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            WBUIActions.UpdateHealth?.Invoke(CurrentHealth / Health);
    }

    [ServerRpc (RequireOwnership =false)]
    void DestroyPlayerServerRPC(ulong id)
    {
        var players = FindObjectsOfType<WBThirdPersonController>();
        foreach (var item in players)
        {
            if(item.NetworkObject.OwnerClientId==id)
            {
                item.NetworkObject.Despawn(true);
            }
        }
    }

}
