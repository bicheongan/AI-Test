using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform player;

    [Header("Pool Settings")]
    [SerializeField] private int maxMonsters = 3;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnDistance = 10f;
    [SerializeField] private float maxSpawnDistance = 20f;
    [SerializeField] private float spawnInterval = 3f;

    private readonly List<GameObject> monsterPool =
        new List<GameObject>();

    private Vector3 playerStartPosition;
    private float spawnTimer;

    private void Start()
    {
        if (monsterPrefab == null || player == null)
        {
            Debug.LogError(
                "MonsterSpawner: Prefab 또는 Player가 연결되지 않았습니다.",
                this
            );

            enabled = false;
            return;
        }

        playerStartPosition = player.position;

        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < maxMonsters; i++)
        {
            GameObject monster =
                Instantiate(monsterPrefab, transform);

            monster.SetActive(false);

            MonsterAI ai = monster.GetComponent<MonsterAI>();

            if (ai != null)
            {
                ai.SetTarget(player);
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

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnMonster();
        }
    }

    private void SpawnMonster()
    {
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
                ai.ResetMonster();
            }

            return;
        }
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        const int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomDirection =
                Random.insideUnitCircle.normalized;

            float distance =
                Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 candidate = playerStartPosition +
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

