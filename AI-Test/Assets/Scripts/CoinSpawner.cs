using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int maxCoins = 5;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 areaSize = new Vector2(40f, 40f);
    [SerializeField] private float spawnHeight = 1f;

    private readonly List<GameObject> coinPool = new List<GameObject>();

    private void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        if (coinPrefab == null)
        {
            Debug.LogError("CoinSpawner: Coin Prefab이 연결되지 않았습니다.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < maxCoins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform);
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

        int activeCount = 0;

        foreach (GameObject coin in coinPool)
        {
            if (coin.activeSelf)
            {
                activeCount++;
            }
        }

        while (activeCount < maxCoins)
        {
            SpawnCoin();
            activeCount++;
        }
    }

    private void SpawnCoin()
    {
        foreach (GameObject coin in coinPool)
        {
            if (!coin.activeSelf)
            {
                Vector3 spawnPosition = new Vector3(
                    Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                    spawnHeight,
                    Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
                );

                coin.transform.position = spawnPosition;
                coin.SetActive(true);
                return;
            }
        }
    }
}

