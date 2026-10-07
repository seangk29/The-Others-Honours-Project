using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public Transform[] patrolPoints;

    public float stopAtDistance = 0.5f;
    public float patrolWaitTime = 2f;

    public NavMeshAgent agent;
    public Animator anim;

    public int currentPatrolIndex;
    public bool isWaiting;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }


    private void Start()
    {
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        Patrol();
        UpdateAnimations();
    }
    void Patrol()
    {
        if (isWaiting) return;

        if (!agent.pathPending && agent.remainingDistance <= stopAtDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }
    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        agent.isStopped = false;
        GoToNextPatrolPoint();
        isWaiting = false;
    }
    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void UpdateAnimations()
    {
        var isMoving = agent.velocity.sqrMagnitude > 0.01f;
        anim.SetBool("isWalking", isMoving);
    }
}
