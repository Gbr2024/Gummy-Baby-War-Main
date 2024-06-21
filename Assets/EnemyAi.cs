
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;
using WeirdBrothers.ThirdPersonController;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    public NavMeshAgent agent;

    public List<Transform> players;

    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    internal Transform nearestPlayer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

   

    private void Update()
    {
        // Find the nearest player
        nearestPlayer = FindNearestPlayer();

        //Check for sight and attack range
        playerInSightRange = nearestPlayer != null && Vector3.Distance(transform.position, nearestPlayer.position) <= sightRange;
        playerInAttackRange = nearestPlayer != null && Vector3.Distance(transform.position, nearestPlayer.position) <= attackRange;

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private Transform FindNearestPlayer()
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var item in players)
        {
            if(item==null)
            {
                players = FindTarget();
                break;
            }
        }
        
        foreach (Transform player in players)
        {
           
            if (transform.TryGetComponent(out HealthManager H)) if (H.isDead) continue;
            if (transform.TryGetComponent(out AIHealth A)) if (A.isDead) continue;


            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = player;
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

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (nearestPlayer != null)
            agent.SetDestination(nearestPlayer.position);
    }

    private void AttackPlayer()
    {
        if (nearestPlayer != null)
        {
            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);
            transform.LookAt(nearestPlayer);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}