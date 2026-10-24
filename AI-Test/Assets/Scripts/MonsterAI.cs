using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Chase Settings")]
    [SerializeField] private float updatePathInterval = 0.2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDistance = 1.5f;

    private NavMeshAgent agent;
    private float pathUpdateTimer;
    private bool hasAttacked;

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
        if (GameManager.Instance == null)
        {
            return;
        }

        if (!agent.isOnNavMesh)
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

        // 플레이어와 몬스터 사이의 거리 계산
        float distanceToPlayer = Vector3.Distance(
            transform.position,
            target.position
        );

        // 공격 거리 안에 들어오면 공격
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

        // GameManager를 통해 플레이어 체력 감소
        GameManager.Instance.DamagePlayer();

        Debug.Log(
            "몬스터 공격! 현재 체력: " +
            GameManager.Instance.CurrentHealth
        );

        // 공격한 몬스터를 풀로 반환
        gameObject.SetActive(false);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
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
