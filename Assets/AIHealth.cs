using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using WeirdBrothers.ThirdPersonController;

public class AIHealth : NetworkBehaviour
{
    [SerializeField] float CurrentHealth;
    [SerializeField] RagdollController ragdollController;
    float Health = 100;

    internal bool isDead = false;
    internal bool isActivated = false;
    [SerializeField] PlayerController playercontroller;

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead || !LobbyManager.Instance.GameHasStarted || ScoreManager.Instance.GameHasFinished || !isActivated) return;

        if (collision.gameObject.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (bullet.isRed == playercontroller.isRed) return;
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
            playercontroller.AddDamage(bullet.damage, NetworkObject.OwnerClientId, bullet.PlayerID);
        }
        if (collision.gameObject.TryGetComponent<Slime>(out Slime slime))
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (!slime.isActive) return;
            if (slime.isRed == playercontroller.isRed) return;
            if (CurrentHealth - slime.Damage <= 0)
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
            playercontroller.AddDamage(slime.Damage, NetworkObject.OwnerClientId, slime.id);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead || !LobbyManager.Instance.GameHasStarted || ScoreManager.Instance.GameHasFinished || !isActivated) return;

        if (other.gameObject.TryGetComponent<Impact>(out Impact impact))
        {
            //Debug.LogError(impact.DamagetoApply);
            if (!NetworkManager.Singleton.IsServer) return;

            if (CurrentHealth - impact.DamagetoApply <= 0 && playercontroller.isRed != impact.isRed)
            {
                isDead = true;
                ScoreManager.Instance.SetKillServerRpc(impact.PlayerID);
            }
            playercontroller.AddDamage(impact.DamagetoApply, NetworkObject.OwnerClientId, impact.PlayerID);
        }

        if (other.gameObject.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (bullet.isRed == playercontroller.isRed) return;
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
            playercontroller.AddDamage(bullet.damage, NetworkObject.OwnerClientId, bullet.PlayerID);
        }
        if (other.gameObject.TryGetComponent<Slime>(out Slime slime))
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (!slime.isActive) return;
            if (slime.isRed == playercontroller.isRed) return;
            if (CurrentHealth - slime.Damage <= 0)
            {
                isDead = true;
                ScoreManager.Instance.SetKillServerRpc(slime.id);
            }
            playercontroller.AddDamage(slime.Damage, NetworkObject.OwnerClientId, slime.id);
        }
    }

    internal void AddDamage(float damage, ulong playerID)
    {
        CurrentHealth -= damage;
        CustomProperties.Instance.currentHealth = CurrentHealth;
        if (CurrentHealth < 0) CurrentHealth = 0;
        if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            WBUIActions.UpdateHealth?.Invoke(CurrentHealth / Health);
            if (CurrentHealth == 0)
            {
                SetKillCam(playerID);
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
        PlayerSetManager.instance.CreateNewAI(playercontroller.AIname,playercontroller.isRed);
    }



    internal void ResetHealth()
    {
        isDead = false;
        CurrentHealth = Health;
        if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            WBUIActions.UpdateHealth?.Invoke(CurrentHealth / Health);
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyPlayerServerRPC(ulong id)
    {
        var players = FindObjectsOfType<WBThirdPersonController>();
        foreach (var item in players)
        {
            if (item.NetworkObject.OwnerClientId == id)
            {
                item.NetworkObject.Despawn(true);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void DestroyAIServerRPC(ulong id)
    {
        var players = FindObjectsOfType<PlayerController>();
        foreach (var item in players)
        {
            if (item.NetworkObject.OwnerClientId == id)
            {
                item.NetworkObject.Despawn(true);
            }
        }
    }
}
