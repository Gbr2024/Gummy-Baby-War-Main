using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using WeirdBrothers.ThirdPersonController;
using DG.Tweening;
using System;

public class SuperGranny : NetworkBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioClip thudSound;

    public List<Transform> players;
    public float speed = 30f;


    //Patroling
    bool walkPointSet;

    //States
    public float attackRange;
    public bool playerInAttackRange;
    bool WorkDone = false;

    public Transform nearestPlayer;

    private Vector3 lastPosition;
    private Vector3 TargetPos;
    Rigidbody rb;


    ulong SetScoreToPlayer;
    bool isTargetRed;

    private void Awake()
    {
        WorkDone = false;
        rb = GetComponent<Rigidbody>();
    }



    private void Start()
    {

        if (!IsServer) return;
        //InvokeRepeating(nameof(findnearestPLayer), 2f, 2f);
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        findnearestPLayer();
        TargetPos = nearestPlayer.position;

        transform.DOMove(TargetPos, 1.75f).OnUpdate(() =>
        {
            if (Vector3.Distance(transform.position, TargetPos) < 5f && animator.GetBool("CompleteFall") != true)
            { 
                animator.SetBool("CompleteFall", true);
                GetComponent<AudioSource>().clip = thudSound;
                GetComponent<AudioSource>().Play() ;

            }
        }).OnComplete(() =>
        {
            killtheTargets();
            Invoke(nameof(DespawnGranny), 2f);
        });
    }

    private void killtheTargets()
    {
        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            if (item.isRed==isTargetRed && Vector3.Distance(item.transform.position,transform.position)<7f)
            {
                //var rb = item.GetComponent<Rigidbody>();
                //Vector3 direction = (nearestPlayer.position - transform.position).normalized;
                //direction.y = 0.0f; // Flatten direction vector on the horizontal plane
                //Vector3 forceDirection = direction.normalized * 125f + Vector3.up * 25f;
                //float forceMagnitude = rb.mass * 15f / .35f;
                //Vector3 force = forceDirection.normalized * forceMagnitude;
                item.BreakCameraClientRPC(item.OwnerClientId, Vector3.zero);
                item.AddDamage(1000f, item.OwnerClientId, SetScoreToPlayer);
                ScoreManager.Instance.SetKillServerRpc(SetScoreToPlayer);
                //WorkDone = true;
                

            }
        }
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if (item.isRed.Value == isTargetRed && Vector3.Distance(item.transform.position, transform.position) < 7f)
            {
                item.AddDamage(1000f);
                ScoreManager.Instance.SetKillServerRpc(SetScoreToPlayer);
                //WorkDone = true;

            }
        }
    }

    private void DespawnGranny()
    {
        Debug.LogError(NetworkObject);
        NetworkObject.Despawn(true);
    }

    internal void SetData(ulong ClientId, bool v)
    {
        SetScoreToPlayer = ClientId;
        isTargetRed = v;
        Debug.LogError("Target Pos" + TargetPos);
        
        
    }

    void findnearestPLayer()
    {
        nearestPlayer = FindNearestPlayer();
    }

    //public Vector3 localVelocity { get; private set; }
    //Vector3 normalizedVelocity;
    //float smoothHor, smoothVer;
    //float smoothingSpeed = 1f;

    //private void Update()
    //{
    //    if (!IsServer || WorkDone) return;

    //    // Find the nearest player
    //    if (nearestPlayer == null)
    //    {
    //        nearestPlayer = FindNearestPlayer();
    //    }
    //    if (nearestPlayer != null)
    //    {
    //        // Check for sight and attack range
    //        playerInAttackRange = Vector3.Distance(transform.position, nearestPlayer.position) <= attackRange;

    //        if (!playerInAttackRange) ChasePlayer();
    //        if (playerInAttackRange) AttackPlayer();
    //    }
    //    else
    //    {
    //        animator.SetFloat("Hor", 0);
    //        animator.SetFloat("Ver", 0);
    //    }
    //}


    private Transform FindNearestPlayer()
    {
        Transform nearest = null;
        players = FindTarget();


        float dis = Mathf.Infinity;

        foreach (var item in players)
        {
            if (item.TryGetComponent(out AIHealth health)) if (health.isDead) continue;
            if (item.TryGetComponent(out HealthManager h)) if (h.isDead) continue;

            float td = Vector3.Distance(transform.position, item.position);
            if (td < dis)
            {
                dis = td;
                nearest = item;
            }
        }

        return nearest;
    }

    private List<Transform> FindTarget()
    {
        List<Transform> ts = new();

        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            ts.Add(item.transform);
        }
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            ts.Add(item.transform);
        }

        return ts;
    }

    //private void ChasePlayer()
    //{
    //    MoveToPosition(nearestPlayer.position);
    //}

    //private void MoveToPosition(Vector3 targetPosition)
    //{
    //    animator.SetFloat("Ver", 1f);
    //    Vector3 direction = (targetPosition - transform.position).normalized;
    //    transform.position += direction * Time.deltaTime *speed; // Speed of movement can be adjusted
    //    // Face the direction of movement
    //    if (direction != Vector3.zero)
    //    {
    //        Quaternion lookRotation = Quaternion.LookRotation(direction);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
    //    }
    //}

    //Vector3 pos;
    //float lastattack = 0;

    //private void AttackPlayer()
    //{
    //    animator.SetFloat("Hor", 0);
    //    animator.SetFloat("Ver", 0);
    //    if (nearestPlayer != null && Time.time - lastattack > 1.5f)
    //    {
    //        lastattack = Time.time;
    //        pos = new Vector3(nearestPlayer.position.x, transform.position.y, nearestPlayer.position.z);
    //        //Make sure enemy doesn't move
    //        transform.LookAt(pos);

    //        animator.SetTrigger("Kick");
    //    }
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, attackRange);
    //    Gizmos.color = Color.yellow;
    //}




    //public void ThrowTheNearestPlayer()
    //{
    //    if (!IsServer) return;
    //    if (nearestPlayer.TryGetComponent(out UnityEngine.AI.NavMeshAgent agent))
    //    {
    //        ThrowAI(nearestPlayer.GetComponent<PlayerController>().AIname);
    //    }
    //    else
    //    {

    //        ThrowplayerServerRpc(nearestPlayer.GetComponent<NetworkObject>().OwnerClientId);
    //    }
    //}

    [ServerRpc]
    public void ThrowplayerServerRpc(ulong playerID)
    {
        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            if (item.OwnerClientId == playerID)
            {
                //var rb = item.GetComponent<Rigidbody>();
                //Vector3 direction = (nearestPlayer.position - transform.position).normalized;
                //direction.y = 0.0f; // Flatten direction vector on the horizontal plane
                //Vector3 forceDirection = direction.normalized * 125f + Vector3.up * 25f;
                //float forceMagnitude = rb.mass * 15f / .35f;
                //Vector3 force = forceDirection.normalized * forceMagnitude;
                item.BreakCameraClientRPC(playerID, Vector3.zero);
                item.AddDamage(1000f, item.OwnerClientId, SetScoreToPlayer);
                ScoreManager.Instance.SetKillServerRpc(SetScoreToPlayer);
                //WorkDone = true;
                Invoke(nameof(DespawnGranny), 2f);

            }
        }
    }

    public void ThrowAI(string playerID)
    {
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if (item.AIname == playerID)
            {
                item.AddDamage(1000f);
                ScoreManager.Instance.SetKillServerRpc(SetScoreToPlayer);
                //WorkDone = true;
                Invoke(nameof(DespawnGranny), 2f);

            }
        }
    }

    
}
