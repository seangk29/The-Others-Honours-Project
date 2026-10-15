using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyState
    {

    Patrol,
    FollowPlayer,
    Attack,
    CaughtPlayer,
    InvestigateNoise,

}
public class EnemyController : MonoBehaviour
{

    public Transform[] patrolPoints;

    public float stopAtDistance = 0.5f;
    public float patrolWaitTime = 2f;

    public float detectionRange = 5f;
    public float viewAngle = 90f;
    public float lostPlayerTime = 3f;
    public float timeSinceLostPlayer;

    public float playerFoundTimer;
    public float playerHasBeenFound;

    public float soundDetectionRange;
    public Transform noise;
    public float distractionTimer;
    public float returnToPost;
    public bool killOldPlayerPos;

    public float attackRange = 1.2f;
    public bool isAttacking;

    public NavMeshAgent agent;
    public Animator anim;

    public int currentPatrolIndex;
    public bool isWaiting;

    public Transform player;

    public EnemyState state = EnemyState.Patrol;


    public PlayerHealth playerHealth;
    public PlayerController playerController;

    private static int isAttackingHash = Animator.StringToHash("Attack");
    private static int isCaughtHash = Animator.StringToHash("Found");
    private static int isChasingHash = Animator.StringToHash("Chasing");
   


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent <PlayerController>();
    }


    private void Start()
    {
        GoToNextPatrolPoint();
    }

    private void Update()
    {

        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (state)
        {
            case EnemyState.Patrol:
                Patrol();
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {

                    playerFoundTimer += Time.deltaTime;

                    if (playerFoundTimer >= playerHasBeenFound)
                    {
                        state = EnemyState.CaughtPlayer;
                        playerFoundTimer = 0;
                    }

                }
                else if (!CanSeePlayer())
                {
                    playerFoundTimer -= Time.deltaTime;

                    if (playerFoundTimer <= 0)
                    {
                        playerFoundTimer = 0;
                    }
                }

                if (distanceToPlayer <= detectionRange && playerController.soundDetection > distanceToPlayer)
                {
                    state = EnemyState.InvestigateNoise;
                }

                break;
            case EnemyState.InvestigateNoise:
                noise = GameObject.FindGameObjectWithTag("noiseMaker").GetComponent<Transform>();
                InvestigateNoise();

                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    playerFoundTimer += Time.deltaTime;

                    if (playerFoundTimer >= playerHasBeenFound)
                    {
                        state = EnemyState.CaughtPlayer;
                        playerFoundTimer = 0;
                    }

                }

               
                break;

            case EnemyState.FollowPlayer:
                FollowPlayer();

                if (distanceToPlayer <= attackRange)
                {
                    state = EnemyState.Attack;
                    StartAttack();
                }

                if (!CanSeePlayer())
                {
                    timeSinceLostPlayer += Time.deltaTime;

                    if (timeSinceLostPlayer >= lostPlayerTime)
                    {
                        state = EnemyState.Patrol;
                        GoToClosestPatrolPoint();
                    }
                }
                else
                {
                    timeSinceLostPlayer = 0f;
                }
                break;

            case EnemyState.Attack:
                Attack();
                if (!isAttacking && distanceToPlayer > attackRange)
                {
                    state = EnemyState.FollowPlayer;
                    agent.isStopped = false;


                }
                isAttacking = false;
                break;

            case EnemyState.CaughtPlayer:
                CaughtPlayer();
                break;

        }

        UpdateAnimations();

        
    }


    void OnAttackAnimationEnd()
    {

        // isAttacking = false;


    }
    void Attack()
    {
        agent.isStopped = true;
        var direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (playerHealth.invul <= 0)
        {
            playerHealth.takeDamage = true;

        }


        if (direction == Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }


    }
    void StartAttack()
    {


        agent.isStopped = true;
        isAttacking = true;


    }

    void CaughtPlayer()
    {
       
        StartCoroutine(Caught());
        killOldPlayerPos = true;

    }

     IEnumerator Caught()
    {
        killOldPlayerPos = true;
        
        timeSinceLostPlayer = 0;

        bool isCaught = state == EnemyState.CaughtPlayer;

        isCaught = true;

        agent.isStopped = true;

        yield return new WaitForSeconds(1.5f);

        

        agent.isStopped = false;
        isCaught = false;

        state = EnemyState.FollowPlayer;

        if (killOldPlayerPos == true)
        {
            ResetInvesitgate();
            killOldPlayerPos = false;
        }

    }
        
    void InvestigateNoise()
    {
        
        agent.SetDestination(noise.position);
        

        if (agent.transform.position.x == noise.position.x )
        {
            distractionTimer += Time.deltaTime;
           

            if (distractionTimer >= returnToPost )
            {
                GoToClosestPatrolPoint();
                state = EnemyState.Patrol;

                playerController.createNoiseTarget = true;
                Destroy(noise.gameObject);
                distractionTimer = 0;
               
            }
        }

      
       
    }

    void ResetInvesitgate()
    {
        playerController.createNoiseTarget = true;
        Destroy(noise.gameObject);
        distractionTimer = 0;
      
    }
    void FollowPlayer()
    {
        agent.SetDestination(player.position);
        agent.speed = 5.5f;

    }
    void Patrol()
    {
        agent.speed = 2.5f;

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

    void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (var i = 0; i < patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        currentPatrolIndex = closestIndex;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
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
        var isMoving = agent.velocity.sqrMagnitude > 0.01f && state == EnemyState.Patrol 
                    || agent.velocity.sqrMagnitude > 0.01f  && state == EnemyState.InvestigateNoise;
        anim.SetBool("isWalking", isMoving);

        bool Attack = state == EnemyState.Attack;
        anim.SetBool(isAttackingHash, Attack);

        bool isCaught = state == EnemyState.CaughtPlayer;
        anim.SetBool(isCaughtHash, isCaught);

        bool isChasing = agent.velocity.sqrMagnitude > 0.01f && state == EnemyState.FollowPlayer;
        anim.SetBool(isChasingHash, isChasing);

        
    }

    private bool CanSeePlayer()
    {
        return IsFacingPlayer() && HasClearPathToPlayer();
    }
    private bool IsFacingPlayer()
    {
        var dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= viewAngle / 2f;
    }    

    private bool HasClearPathToPlayer()
    {
        var dirToPlayer = player.position - transform.position;
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }

        return true;
    }
}
