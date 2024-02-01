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


    private void Awake()
    {
        CurrentHealth = Health;
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
       
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
        if(NetworkObject.OwnerClientId==NetworkManager.Singleton.LocalClientId)
            WBUIActions.UpdateHealth?.Invoke(CurrentHealth / Health);

        if (CurrentHealth == 0)
        {
            ragdollController.SetToAll(true);
            Invoke(nameof(resetPlayer), 5f);
            isDead = true;
        }
    }

    void resetPlayer()
    {
        ragdollController.SetToAll(false);
        ResetHealth();
        if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            transform.position = CustomProperties.Instance.isRed ? PlayerSetManager.instance.RedCribs[Random.Range(0, 3)].position : PlayerSetManager.instance.BlueCribs[Random.Range(0, 3)].position;
    }

    internal void ResetHealth()
    {
        isDead = false;
        CurrentHealth = Health;
        if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            WBUIActions.UpdateHealth?.Invoke(CurrentHealth / Health);
    }

}
