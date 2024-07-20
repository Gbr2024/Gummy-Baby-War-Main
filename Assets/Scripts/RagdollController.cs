using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class RagdollController : NetworkBehaviour
{
    private Animator animator;
    [SerializeField] private List<Rigidbody> rigidbodies=new();
    [SerializeField] private List<Collider> colliders=new();

    [SerializeField]private bool ragdollActive = false;
    private void Awake()
    {
       
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        

    }


    void Start()
    {
        animator = GetComponent<Animator>();

        rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
        colliders = GetComponentsInChildren<Collider>().ToList();
        rigidbodies.Remove(GetComponent<Rigidbody>());
        colliders.Remove(GetComponent<Collider>());
        // Set the rigidbodies and colliders to kinematic at the start
        SetRagdollState(false);
    }

    

    internal void ToggleRagdoll()
    {
        ragdollActive = !ragdollActive;
        GetComponent<Rigidbody>().isKinematic = ragdollActive;
        GetComponent<Collider>().enabled = !ragdollActive;
        // Toggle between ragdoll and animation
        if (ragdollActive)
        {
            SetRagdollState(true);
        }
        else
        {
            SetRagdollState(false);
            // Optionally, reset forces and velocities when switching back to animation
            ResetRigidbodyVelocities();
        }
    }

    internal void SetToAll(bool b)
    {
        SetRagdollServerRpc(NetworkObject.OwnerClientId,b);
    }
    
    internal void SetToAll(string name, bool b)
    {
        SetAIRagdollServerRpc(name,b);
    }

    

    [ServerRpc(RequireOwnership =false)]
    void SetRagdollServerRpc(ulong id,bool b)
    {
        SetRagdollClientRpc(id,b);
    }
    
    [ServerRpc(RequireOwnership =false)]
    void SetAIRagdollServerRpc(string aIname,bool b)
    {
        SetAIRagdollClientRpc(aIname, b);
    }

    [ClientRpc]
    void SetRagdollClientRpc(ulong id,bool b)
    {
        if (NetworkObject.OwnerClientId == id)
            SetRagdollState(b);
    }
    
    [ClientRpc]
    void SetAIRagdollClientRpc(string id,bool b)
    {
        GetComponent<PlayerController>().DisableAgent();
        if (GetComponent<PlayerController>().AIname == id)
            SetRagdollState(b);
    }

    
    void SetRagdollState(bool active)
    {
        // Set the animator and colliders state
        animator.enabled = !active;
        //if(!active)
        //    GetComponent<Collider>().enabled = false;

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !active;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = active;
        }
        ResetRigidbodyVelocities();

    }

    void ResetRigidbodyVelocities()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            if(!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
        }
    }


   

    



    // Apply force to the ragdoll
    private void OnCollisionEnter(Collision collision)
    {
       
        //if (collision.gameObject.tag=="Weapon")
        //    ToggleRagdoll();
    }


}
