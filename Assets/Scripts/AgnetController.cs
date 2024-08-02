using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class AgnetController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    public float maxSpeed = 5.0f; // Maximum agent speed
    public float NormalSpeed = 1.0f; // Maximum agent speed
    public float accelerationRate = 0.5f; // Rate of acceleration
    public float SlowWalk = .35f;
    [SerializeField] float Speed;
    public Func<bool> CheckFortheRestriction = () => false;
    [SerializeField] PlayerController playerController;
    internal Transform CurrentTarget;
    internal Vector3 destination;
    public bool isMoving;

    NavMeshHit hit;
    float distanceToDestination;
    float height, radius;
    private void Awake()
    {
        Speed = NormalSpeed;
        playerController = GetComponent<PlayerController>();
    }
    private void Start()
    {
        height = agent.height;
        radius = agent.radius;
        agent.autoTraverseOffMeshLink = false;
    }

    Vector3 velocity;

    void Update()
    {

        if (playerController == null) playerController = GetComponent<PlayerController>();

        if (CurrentTarget != null)
        {
            transform.LookAt(CurrentTarget);
            float distanceToDestination = Vector3.Distance(transform.position, CurrentTarget.position);

            if (distanceToDestination > agent.stoppingDistance && !isMoving)
            {
                agent.SetDestination(destination);
                Recalculate();
                isMoving = true;
            }
        }

        if (isMoving)
        {
            float distanceToDestination = Vector3.Distance(transform.position, destination);
            agent.speed = Mathf.MoveTowards(agent.speed, Speed, accelerationRate * Time.deltaTime);

            velocity = transform.InverseTransformDirection(agent.velocity);
            animator.SetFloat("Hor", velocity.x);
            animator.SetFloat("Ver", velocity.z);

            if (distanceToDestination <= agent.stoppingDistance)
            {
                agent.isStopped = true;
                isMoving = false;
                animator.SetFloat("Hor", 0.0f);
                animator.SetFloat("Ver", 0.0f);
                agent.speed = 0;
                return;
            }
        }
    }

    internal void SetDestinationandMove(Transform t, Vector3 dest)
    {
        StartCoroutine(SetDestination(t, dest));
    }

    IEnumerator SetDestination(Transform t, Vector3 dest)
    {
        isMoving = false;
        yield return new WaitForSeconds(.25f);
        CurrentTarget = t;
        destination = dest;
        agent.SetDestination(destination);
        isMoving = true;
    }

    public void DisableAgent()
    {
        agent.enabled = false;
        if (agent.isActiveAndEnabled) agent.isStopped = true;
        enabled = false;
    }

    private void OnEnable()
    {
        agent.enabled = true;
        Recalculate();
    }

    internal void ResetRadius()
    {
        agent.height = height;
        agent.radius = radius;
    }
    internal void SetRadius(float f)
    {
        agent.height = f;
        agent.radius = f;
    }

    internal void Recalculate()
    {
        if (destination != Vector3.zero)
        {
            agent.ResetPath();
            agent.SetDestination(destination);
        }
    }

    internal void SetSlowSpeed()
    {
        Speed = SlowWalk;
    }
    internal void SetFastSpeed()
    {
        Speed = maxSpeed;
    }
    internal void setNormalSpeed()
    {
        Speed = NormalSpeed;
    }
}
