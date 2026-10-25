using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform player;

    [Header("Monster Count")]
    [SerializeField, Min(1)] private int maxMonsters = 10;
    [SerializeField, Min(1)] private int startingMonsters = 3;

    [Header("Spawn Settings")]
    [SerializeField, Min(0f)] private float minSpawnDistance = 10f;
    [SerializeField, Min(0f)] private float maxSpawnDistance = 20f;
    [SerializeField, Min(0.1f)] private float spawnInterval = 3f;

    [Header("Difficulty")]
    [SerializeField, Min(0.1f)] private float difficultyInterval = 15f;
    [SerializeField, Min(0f)] private float speedIncrease = 0.5f;
    [SerializeField, Min(0f)] private float speedBelowPlayer = 0.5f;

    private readonly List<GameObject> monsterPool =
        new List<GameObject>();

    private Vector3 playerStartPosition;
    private PlayerController playerController;

    private int currentMonsterLimit;
    private float currentMonsterSpeed;

    private float spawnTimer;
    private float difficultyTimer;
    private bool hasStartedSpawning;

    private void Start()
    {
        if (monsterPrefab == null || player == null)
        {
            Debug.LogError(
                "MonsterSpawner: Prefab 또는 Player가 없습니다.",
                this
            );

            enabled = false;
            return;
        }

        playerController = player.GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError(
                "MonsterSpawner: PlayerController가 없습니다.",
                this
            );

            enabled = false;
            return;
        }

        maxMonsters = Mathf.Max(1, maxMonsters);

        startingMonsters = Mathf.Clamp(
            startingMonsters,
            1,
            maxMonsters
        );

        maxSpawnDistance = Mathf.Max(
            maxSpawnDistance,
            minSpawnDistance
        );

        playerStartPosition = player.position;
        currentMonsterLimit = startingMonsters;

        MonsterAI prefabAI = monsterPrefab.GetComponent<MonsterAI>();

        if (prefabAI == null)
        {
            Debug.LogError(
                "MonsterSpawner: MonsterAI가 없습니다.",
                this
            );

            enabled = false;
            return;
        }

        currentMonsterSpeed = Mathf.Min(
            prefabAI.InitialMoveSpeed,
            GetMaximumMonsterSpeed()
        );

        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < maxMonsters; i++)
        {
            GameObject monster = Instantiate(
                monsterPrefab,
                transform
            );

            monster.SetActive(false);

            MonsterAI ai = monster.GetComponent<MonsterAI>();

            if (ai != null)
            {
                ai.SetTarget(player);
                ai.SetMoveSpeed(currentMonsterSpeed);
            }

            monsterPool.Add(monster);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        if (!hasStartedSpawning)
        {
            hasStartedSpawning = true;

            for (int i = 0; i < startingMonsters; i++)
            {
                SpawnMonster();
            }
        }

        UpdateDifficulty();

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer -= spawnInterval;
            SpawnMonster();
        }
    }

    private void UpdateDifficulty()
    {
        difficultyTimer += Time.deltaTime;

        if (difficultyTimer < difficultyInterval)
        {
            return;
        }

        difficultyTimer -= difficultyInterval;

        // 1. 몬스터 수 증가
        if (currentMonsterLimit < maxMonsters)
        {
            currentMonsterLimit++;

            // 증가한 슬롯에 몬스터 1마리 생성 시도
            SpawnMonster();
        }

        // 2. 몬스터 속도 증가
        // 몬스터 수와 관계없이 동시에 진행
        currentMonsterSpeed = Mathf.Min(
            currentMonsterSpeed + speedIncrease,
            GetMaximumMonsterSpeed()
        );

        // 이미 생성된 몬스터와 풀의 몬스터 모두 적용
        ApplySpeedToAllMonsters();

        Debug.Log(
            $"난이도 상승! 몬스터 제한: {currentMonsterLimit}, " +
            $"몬스터 속도: {currentMonsterSpeed:F1}"
        );
    }

    private float GetMaximumMonsterSpeed()
    {
        return Mathf.Max(
            0f,
            playerController.BaseMoveSpeed - speedBelowPlayer
        );
    }

    private void ApplySpeedToAllMonsters()
    {
        foreach (GameObject monster in monsterPool)
        {
            MonsterAI ai = monster.GetComponent<MonsterAI>();

            if (ai != null)
            {
                ai.SetMoveSpeed(currentMonsterSpeed);
            }
        }
    }

    private void SpawnMonster()
    {
        if (GetActiveMonsterCount() >= currentMonsterLimit)
        {
            return;
        }

        foreach (GameObject monster in monsterPool)
        {
            if (monster.activeSelf)
            {
                continue;
            }

            if (!TryGetSpawnPosition(out Vector3 spawnPosition))
            {
                return;
            }

            monster.transform.position = spawnPosition;
            monster.SetActive(true);

            MonsterAI ai = monster.GetComponent<MonsterAI>();

            if (ai != null)
            {
                ai.SetMoveSpeed(currentMonsterSpeed);
                ai.ResetMonster();
            }

            return;
        }
    }

    private int GetActiveMonsterCount()
    {
        int count = 0;

        foreach (GameObject monster in monsterPool)
        {
            if (monster.activeSelf)
            {
                count++;
            }
        }

        return count;
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        const int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomDirection =
                Random.insideUnitCircle.normalized;

            float distance = Random.Range(
                minSpawnDistance,
                maxSpawnDistance
            );

            Vector3 candidate =
                playerStartPosition +
                new Vector3(
                    randomDirection.x * distance,
                    0f,
                    randomDirection.y * distance
                );

            if (NavMesh.SamplePosition(
                candidate,
                out NavMeshHit hit,
                2f,
                NavMesh.AllAreas))
            {
                float actualDistance = Vector3.Distance(
                    playerStartPosition,
                    hit.position
                );

                if (actualDistance >= minSpawnDistance)
                {
                    spawnPosition = hit.position;
                    return true;
                }
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }
}
