using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Chase Settings")]
    [SerializeField] private float updatePathInterval = 0.2f;

    private NavMeshAgent agent;
    private float pathUpdateTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                target = player.transform;
            }
        }

        if (target == null)
        {
            Debug.LogError(
                "MonsterAI: Player Target이 연결되지 않았습니다.",
                this
            );

            enabled = false;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        bool isPlaying =
            GameManager.Instance.CurrentState == GameState.Playing;

        if (!agent.isOnNavMesh)
        {
            return;
        }

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

        pathUpdateTimer -= Time.deltaTime;

        if (pathUpdateTimer <= 0f)
        {
            agent.SetDestination(target.position);
            pathUpdateTimer = updatePathInterval;
        }
    }
}

