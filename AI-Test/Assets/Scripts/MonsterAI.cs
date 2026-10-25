using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float initialMoveSpeed = 2f;
    [SerializeField, Min(0.01f)] private float updatePathInterval = 0.2f;

    [Header("Attack")]
    [SerializeField, Min(0f)] private float attackDistance = 1.5f;

    private NavMeshAgent agent;
    private float pathUpdateTimer;
    private bool hasAttacked;

    public float InitialMoveSpeed => initialMoveSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError(
                "MonsterAI: Target이 연결되지 않았습니다.",
                this
            );

            enabled = false;
        }
    }

    private void OnEnable()
    {
        pathUpdateTimer = 0f;
        hasAttacked = false;
    }

    private void Update()
    {
        if (GameManager.Instance == null || !agent.isOnNavMesh)
        {
            return;
        }

        bool isPlaying =
            GameManager.Instance.CurrentState == GameState.Playing;

        if (!isPlaying)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        if (target == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            target.position
        );

        if (!hasAttacked && distanceToPlayer <= attackDistance)
        {
            AttackPlayer();
            return;
        }

        pathUpdateTimer -= Time.deltaTime;

        if (pathUpdateTimer <= 0f)
        {
            agent.SetDestination(target.position);
            pathUpdateTimer = updatePathInterval;
        }
    }

    private void AttackPlayer()
    {
        if (hasAttacked)
        {
            return;
        }

        hasAttacked = true;

        GameManager.Instance.DamagePlayer();

        Debug.Log(
            "몬스터 공격! 현재 체력: " +
            GameManager.Instance.CurrentHealth
        );

        gameObject.SetActive(false);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetMoveSpeed(float speed)
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        agent.speed = Mathf.Max(0f, speed);
    }

    public void ResetMonster()
    {
        pathUpdateTimer = 0f;
        hasAttacked = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }
    }

    private void OnDisable()
    {
        ResetMonster();
    }
}
