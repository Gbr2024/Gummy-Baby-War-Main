using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private List<Rigidbody> rigidbodies=new();
    [SerializeField] private List<Collider> colliders=new();
    public KeyCode toggleKey = KeyCode.Space;

    [SerializeField]private bool ragdollActive = false;

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

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleRagdoll();
        }
        if(Input.GetMouseButtonDown(1))
        {
            ToggleRagdoll();
        }
    }

    void ToggleRagdoll()
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

    void SetRagdollState(bool active)
    {
        // Set the animator and colliders state
        animator.enabled = !active;

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
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // Apply force to the ragdoll
    private void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.tag=="Weapon")
            ToggleRagdoll();
    }


}
