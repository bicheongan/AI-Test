using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Pool")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField, Min(1)] private int maxCoins = 5;

    [Header("Spawn Area")]
    [SerializeField]
    private Vector2 areaSize =
        new Vector2(40f, 40f);

    [SerializeField] private float spawnHeight = 1f;

    [Header("Obstacle Check")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField, Min(0.01f)] private float checkRadius = 0.7f;
    [SerializeField, Min(1)] private int maxSpawnAttempts = 30;

    private readonly List<GameObject> coinPool =
        new List<GameObject>();

    private void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        if (coinPrefab == null)
        {
            Debug.LogError(
                "CoinSpawner: Coin Prefab이 연결되지 않았습니다.",
                this
            );

            enabled = false;
            return;
        }

        for (int i = 0; i < maxCoins; i++)
        {
            GameObject coin = Instantiate(
                coinPrefab,
                transform
            );

            coin.SetActive(false);
            coinPool.Add(coin);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        foreach (GameObject coin in coinPool)
        {
            if (coin.activeSelf)
            {
                continue;
            }

            // 생성 가능한 위치가 없으면 이번 프레임은 종료
            if (!TryGetSpawnPosition(out Vector3 spawnPosition))
            {
                break;
            }

            coin.transform.position = spawnPosition;
            coin.SetActive(true);
        }
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 candidate = new Vector3(
                transform.position.x +
                    Random.Range(-areaSize.x / 2f, areaSize.x / 2f),

                spawnHeight,

                transform.position.z +
                    Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            bool blocked = Physics.CheckSphere(
                candidate,
                checkRadius,
                obstacleLayer,
                QueryTriggerInteraction.Ignore
            );

            if (!blocked)
            {
                spawnPosition = candidate;
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = new Vector3(
            transform.position.x,
            spawnHeight,
            transform.position.z
        );

        Gizmos.DrawWireCube(
            center,
            new Vector3(areaSize.x, 0.1f, areaSize.y)
        );
    }
}
