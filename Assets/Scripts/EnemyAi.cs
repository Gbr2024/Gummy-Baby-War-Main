using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;
using WeirdBrothers.ThirdPersonController;
using System.Collections;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] AIHealth health;
    [SerializeField] Transform Chest; // New field for enemy's chest height
    [SerializeField] bool DebugMessage;
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
    public bool isKicked;

    public Transform nearestPlayer;

    private Vector3 lastPosition;
    private float stuckTimer;
    private float stuckThreshold = 2f; // Time in seconds to consider the agent stuck
    private float stuckDistance = 0.5f;

    private void Awake()
    {
        isKicked = false;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        sightRange = ItemReference.Instance.SightRange;
        attackRange = ItemReference.Instance.AttackRange;
        if (!playerController.IsServer) return;
        InvokeRepeating(nameof(findnearestPLayer), 2f, 2f);
    }

    void findnearestPLayer()
    {
        nearestPlayer = FindNearestPlayer();
    }

    private void Update()
    {
        if (health.isDead) return;
        if (!playerController.IsServer) return;

        // Find the nearest player
        if (nearestPlayer == null)
        {
            nearestPlayer = FindNearestPlayer();
        }



        if (nearestPlayer != null)
        {
            // Check for sight and attack range
            playerInSightRange = Vector3.Distance(transform.position, nearestPlayer.position) <= sightRange;
            playerInAttackRange = Vector3.Distance(transform.position, nearestPlayer.position) <= attackRange;

            if (playerInAttackRange) CanSeePlayer = CheckCanSeePlayer(nearestPlayer);
            else playerInAttackRange = false;

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (!playerInAttackRange && playerInSightRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange && CanSeePlayer) AttackPlayer();
            if (playerInAttackRange && !CanSeePlayer) MoveToLOSPosition();
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
        if (isKicked) return;
        if (DebugMessage) Debug.Log("Moving to LOS Position");

        // Calculate the LOS position without obstacles
        Vector3 directionToPlayer = (nearestPlayer.position - Chest.position).normalized;
        float maxDistance = attackRange; // Adjust as needed based on your game design

        RaycastHit hit;
        Vector3 LOSPosition = transform.position;

        // Perform raycast from enemy's chest to player's chest
        if (Physics.Raycast(Chest.position, directionToPlayer, out hit, maxDistance))
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
        Vector3 directionToPlayer = (player.position - Chest.position).normalized;

        // Adjusted raycast to check from chest height to player's chest height
        Vector3 rayStart = Chest.position;
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
        foreach (var item in players)
        {
            if (item == null)
            {
                players = FindTarget();
                break;
            }
        }
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
            if (item.isRed != playerController.isRed.Value)
                ts.Add(item.transform);
        }
        foreach (var item in FindObjectsOfType<PlayerController>())
        {
            if (item.isRed.Value != playerController.isRed.Value)
                ts.Add(item.transform);
        }
        return ts;
    }

    internal void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (isKicked) return;

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
        if (isKicked) return;

        if (nearestPlayer != null)
        {
            if (DebugMessage) Debug.LogError("Chasing");
            agent.isStopped = false;
            agent.SetDestination(nearestPlayer.position);
        }
    }

    private void AttackPlayer()
    {
        if (isKicked) return;
        if (nearestPlayer != null)
        {
            //Make sure enemy doesn't move
            transform.LookAt(nearestPlayer);
            if (DebugMessage) Debug.LogError("Attacking");
            agent.isStopped = true;
            agent.SetDestination(transform.position);
            agent.velocity = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public IEnumerator ResetKick()
    {
        yield return new WaitForSeconds(5f);
        agent.isStopped = false;
        isKicked = false;
    }
}
