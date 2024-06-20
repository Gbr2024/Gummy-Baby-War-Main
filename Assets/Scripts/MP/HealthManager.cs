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
        if (isDead || !LobbyManager.Instance.GameHasStarted || ScoreManager.Instance.GameHasFinished || !isActivated) return;
       
        if (collision.gameObject.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (bullet.isRed == controller.isRed) return;
            if (CurrentHealth - bullet.damage <= 0)
            {
                isDead = true;
                if (!bullet.isAI)
                {
                    ScoreManager.Instance.SetKillServerRpc(bullet.PlayerID);
                }
                else
                {
                    ScoreManager.Instance.SetKillServerRpc(bullet.AIname);
                }
            }

            if (!bullet.isAI)
                controller.AddDamage(bullet.damage, NetworkObject.OwnerClientId, bullet.PlayerID);
            else
                controller.AddDamage(bullet.damage, NetworkObject.OwnerClientId, bullet.AIname, true);
        }
        if (collision.gameObject.TryGetComponent<Slime>(out Slime slime))
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (!slime.isActive) return;
            if (slime.isRed == controller.isRed) return;
            if (CurrentHealth - slime.Damage <= 0)
            {
                isDead = true;
                ScoreManager.Instance.SetKillServerRpc(slime.id); 
            }
            controller.AddDamage(slime.Damage, NetworkObject.OwnerClientId, slime.id);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead || !LobbyManager.Instance.GameHasStarted || ScoreManager.Instance.GameHasFinished || !isActivated) return;

        if (other.gameObject.TryGetComponent<Impact>(out Impact impact))
        {
            //Debug.LogError(impact.DamagetoApply);
            if (!NetworkManager.Singleton.IsServer) return;

            if (CurrentHealth - impact.DamagetoApply <= 0 && controller.isRed!=impact.isRed)
            {
                isDead = true;
                ScoreManager.Instance.SetKillServerRpc(impact.PlayerID);
            }
            controller.AddDamage(impact.DamagetoApply, NetworkObject.OwnerClientId, impact.PlayerID);
        }
    }

    internal void AddDamage(float damage, ulong playerID, string AIname, bool isAI = false)
    {
        CurrentHealth -= damage;
        CustomProperties.Instance.currentHealth = CurrentHealth;
        if (CurrentHealth < 0) CurrentHealth = 0;
        if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            WBUIActions.UpdateHealth?.Invoke(CurrentHealth / Health);
            if (CurrentHealth == 0)
            {
                if (!isAI)
                    SetKillCam(playerID);
                else
                    SetKillCamAI(AIname);
                controller.SetScope(false);
                WBUIActions.isPlayerActive = false;
                WBUIActions.EnableTouch?.Invoke(false);
                Invoke(nameof(resetPlayer), 5f);
            }
        }

        if (CurrentHealth == 0)
        {
            GetComponent<ClientNetworkTransform>().enabled = false;
            ragdollController.SetToAll(true);
            isDead = true;
        }
    }

    private void SetKillCamAI(string aIname)
    {
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if (item.AIname == aIname)
                PlayerSetManager.instance.setKillCam(item.transform);
        }
    }

    private void SetKillCam(ulong playerID)
    {
        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            if (item.NetworkObject.OwnerClientId == playerID)
                PlayerSetManager.instance.setKillCam(item.transform);
        } 
    }

    void resetPlayer()
    {
        if (ScoreManager.Instance.GameHasFinished) return;
        //ragdollController.SetToAll(false);
        //ResetHealth();
        //if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        //    transform.position = CustomProperties.Instance.isRed ? PlayerSetManager.instance.RedCribs[Random.Range(0, 3)].position : PlayerSetManager.instance.BlueCribs[Random.Range(0, 3)].position;

        DestroyPlayerServerRPC(NetworkObject.OwnerClientId);
        if (PlayerPrefs.GetInt("WeaponSelected", 0) == 0)
            PlayerSetManager.instance.SpinTheWheel();
        else
            PlayerSetManager.instance.spawnWithoutWheel();
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
