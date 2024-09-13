using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using WeirdBrothers.ThirdPersonController;

public class Granny : NetworkBehaviour
{
    [SerializeField] bool DebugMessage;
    [SerializeField] Animator animator;
    public NavMeshAgent agent;

    public List<Transform> players;

    public LayerMask whatIsGround;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public bool CanSeePlayer;

    public Transform nearestPlayer;

    private Vector3 lastPosition;
    private float stuckTimer;
    private float stuckThreshold = 2f; // Time in seconds to consider the agent stuck
    private float stuckDistance = 0.5f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        
        if (!IsServer) return;
        InvokeRepeating(nameof(findnearestPLayer), 2f, 2f);
        Invoke(nameof(DespawnGranny), 119f);

    }

    

    private void DespawnGranny()
    {
        NetworkObject.Despawn(true);
    }

    void findnearestPLayer()
    {
        nearestPlayer = FindNearestPlayer();
    }

    public Vector3 localVelocity { get; private set; }
    Vector3 normalizedVelocity;
    float smoothHor, smoothVer;
    float smoothingSpeed = 1f;

    private void Update()
    {
        if (!IsServer) return;

        // Find the nearest player
        if (nearestPlayer == null)
        {
            nearestPlayer = FindNearestPlayer();
        }

        localVelocity = transform.InverseTransformDirection(agent.velocity);
        normalizedVelocity = localVelocity.normalized;

        // Smooth transitions for animation parameters
        smoothHor = Mathf.Lerp(animator.GetFloat("Hor"), normalizedVelocity.x, Time.deltaTime * smoothingSpeed);
        smoothVer = Mathf.Lerp(animator.GetFloat("Ver"), normalizedVelocity.z, Time.deltaTime * smoothingSpeed);

        // Set animator parameters
        animator.SetFloat("Hor", smoothHor);
        animator.SetFloat("Ver", smoothVer);


        if (nearestPlayer != null)
        {
            // Check for sight and attack range
            playerInSightRange = Vector3.Distance(transform.position, nearestPlayer.position) <= sightRange;
            playerInAttackRange = Vector3.Distance(transform.position, nearestPlayer.position) <= attackRange;

            if (playerInAttackRange) CanSeePlayer = CheckCanSeePlayer(nearestPlayer);
            else playerInAttackRange = false;

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (!playerInAttackRange && playerInSightRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
        else
        {
            Patroling();
        }
    }

    private void CheckIfStuck()
    {
        if (Vector3.Distance(transform.position, agent.destination) < 2f || agent.isStopped) return;

        float distance = Vector3.Distance(transform.position, lastPosition);

        if (distance < stuckDistance)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                if (DebugMessage) Debug.Log("Agent is stuck, taking action to move");
                RecalculatePath();
                stuckTimer = 0f; // Reset the stuck timer
            }
        }
        else
        {
            stuckTimer = 0f; // Reset the stuck timer if the agent is moving
        }

        lastPosition = transform.position;
    }

    private void RecalculatePath()
    {
        Patroling();
    }

    private void MoveToLOSPosition()
    {
        if (DebugMessage) Debug.Log("Moving to LOS Position");

        // Calculate the LOS position without obstacles
        Vector3 directionToPlayer = (nearestPlayer.position - transform.position).normalized;
        float maxDistance = attackRange; // Adjust as needed based on your game design

        RaycastHit hit;
        Vector3 LOSPosition = transform.position;

        // Perform raycast from enemy's chest to player's chest
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, maxDistance))
        {
            if (hit.transform.CompareTag("Player"))
            {
                LOSPosition = hit.point;
            }
            else
            {
                // If hit something other than player, calculate position just before hitting obstacle
                LOSPosition = hit.point - directionToPlayer * 1f; // Adjust offset as needed
            }
        }
        else
        {
            LOSPosition = nearestPlayer.position;
        }

        // Check if LOSPosition is on NavMesh and within attack range
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(LOSPosition, out navHit, 2.0f, NavMesh.AllAreas))
        {
            // Check if the LOS position allows seeing the player
            if (CheckCanSeePlayer(nearestPlayer, LOSPosition))
            {
                agent.isStopped = false;
                agent.SetDestination(navHit.position);
            }
            else
            {
                if (DebugMessage) Debug.Log("Cannot see player from LOS position, continue patrolling");
                Patroling(); // Continue patrolling if LOS position does not allow seeing the player
            }
        }
        else
        {
            if (DebugMessage) Debug.Log("No valid LOS position found, continue patrolling");
            Patroling(); // Continue patrolling if LOS position is not valid
        }
    }

    private bool CheckCanSeePlayer(Transform player)
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Adjusted raycast to check from chest height to player's chest height
        Vector3 rayStart = transform.position;
        Vector3 rayEnd = player.position + Vector3.up * 1.25f; // Adjust this offset to player's chest height

        if (Physics.Raycast(rayStart, (rayEnd - rayStart).normalized, out hit, sightRange))
        {
            if (hit.transform == player)
                return true;
        }
        return false;
    }
    private bool CheckCanSeePlayer(Transform player, Vector3 LOSPosition)
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - LOSPosition).normalized;

        // Raycast from LOSPosition to player's chest height
        Vector3 rayStart = LOSPosition;
        Vector3 rayEnd = player.position + Vector3.up * 1.25f; // Adjust this offset to player's chest height

        if (Physics.Raycast(rayStart, (rayEnd - rayStart).normalized, out hit, sightRange))
        {
            if (hit.transform == player)
                return true;
        }
        return false;
    }

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
        //foreach (var item in FindObjectsOfType<PlayerController>())
        //{
        //    ts.Add(item.transform);
        //}
        return ts;
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.isStopped = false;
            agent.SetDestination(walkPoint);
        }

        float distanceToWalkPoint = Vector3.Distance(transform.position, walkPoint);

        //Walkpoint reached
        if (distanceToWalkPoint < 5f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        Vector3 randomPoint;
        NavMeshHit hit;

        do
        {
            float randomX = Random.Range(ItemReference.Instance.dropMin.x, ItemReference.Instance.DropMax.x);
            float randomZ = Random.Range(ItemReference.Instance.dropMin.y, ItemReference.Instance.DropMax.y);
            randomPoint = new Vector3(randomX, transform.position.y, randomZ);

            // Check if the random point is on the NavMesh
        } while (!NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas));

        walkPoint = hit.position;
        walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (nearestPlayer != null)
        {
            //if (DebugMessage) Debug.LogError("Chasing");
            agent.isStopped = false;
            agent.SetDestination(nearestPlayer.position);
        }
    }

    Vector3 pos;
    float lastattack = 0;

    private void AttackPlayer()
    {
        if (nearestPlayer != null && Time.time-lastattack>1.5f)
        {
            lastattack = Time.time;
            pos = new Vector3(nearestPlayer.position.x, transform.position.y, nearestPlayer.position.z);
            //Make sure enemy doesn't move
            transform.LookAt(pos);
            agent.isStopped = true;
            agent.SetDestination(transform.position);
            agent.velocity = Vector3.zero;

            animator.SetTrigger("Kick");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }


   

    public void ThrowTheNearestPlayer()
    {
        if (!IsServer) return;
        if(nearestPlayer.TryGetComponent(out NavMeshAgent agent))
        {
            ThrowAI(nearestPlayer.GetComponent<PlayerController>().AIname);
        }
        else
        {

            ThrowplayerServerRpc(nearestPlayer.GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    [ServerRpc]
    public void ThrowplayerServerRpc(ulong playerID)
    {
        foreach (var item in FindObjectsOfType<WBThirdPersonController>())
        {
            if(item.OwnerClientId== playerID)
            {
                var rb = item.GetComponent<Rigidbody>();
                Vector3 direction = (nearestPlayer.position - transform.position).normalized;
                direction.y = 0.0f; // Flatten direction vector on the horizontal plane
                Vector3 forceDirection = direction.normalized * 50f + Vector3.up * 25f;
                float forceMagnitude = rb.mass*15f/.35f;
                Vector3 force = forceDirection.normalized * forceMagnitude;
               
                item.BreakCameraClientRPC(playerID, force);
            }
        }
    }

    public void ThrowAI(string playerID)
    {
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if (item.AIname == playerID)
            {
                string name = item.AIname;
                StartCoroutine(ResetAI(name));
            }
        }
    }

    public IEnumerator ResetAI(string name)
    {
        PlayerController player = null;
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if (item.AIname == name)
            {
                player = item;
                break; // Exit the loop once the player is found
            }
        }

        if (player == null)
        {
            Debug.LogError("Player not found");
            yield break;
        }

        var rb = player.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        player.GetEnemyAi.enabled = false;
        player.GetComponent<NavMeshAgent>().isStopped = true;
        player.GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSeconds(0.25f);

        Vector3 direction = (nearestPlayer.position - transform.position).normalized;
        direction.y = 0.0f; // Flatten direction vector on the horizontal plane

        Debug.LogError($"Direction: {direction}");

        Vector3 horizontalForce = direction * 50f; // Adjust the force as needed
        Vector3 upwardForce = Vector3.up * 25f;
        Vector3 combinedForce = horizontalForce + upwardForce;


        float forceMagnitude = rb.mass * 15f / 0.35f;
        Vector3 force = combinedForce.normalized * forceMagnitude;

        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.1f);
        yield return new WaitForSeconds(0.25f);
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.05f);

        rb.isKinematic = true;
        player.GetEnemyAi.enabled = true;
        player.GetComponent<NavMeshAgent>().enabled = true;
        player.GetEnemyAi.agent.SetDestination(player.transform.position);
        player.GetEnemyAi.Patroling();
    }
}
