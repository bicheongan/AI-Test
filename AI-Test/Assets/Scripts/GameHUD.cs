using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text coinCountText;

    private GameManager subscribedManager;

    private void Start()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "GameHUD: GameManager를 찾을 수 없습니다.",
                this
            );
            return;
        }

        subscribedManager = GameManager.Instance;

        subscribedManager.OnSurvivalTimeChanged += UpdateSurvivalTime;
        subscribedManager.OnCoinCountChanged += UpdateCoinCount;

        UpdateSurvivalTime(subscribedManager.SurvivalTime);
        UpdateCoinCount(subscribedManager.CoinCount);
    }

    private void OnDisable()
    {
        if (subscribedManager == null)
        {
            return;
        }

        subscribedManager.OnSurvivalTimeChanged -= UpdateSurvivalTime;
        subscribedManager.OnCoinCountChanged -= UpdateCoinCount;

        subscribedManager = null;
    }

    private void UpdateSurvivalTime(float time)
    {
        if (survivalTimeText == null)
        {
            return;
        }

        int totalSeconds = Mathf.FloorToInt(time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        survivalTimeText.text =
            $"Alive Time {minutes:00}:{seconds:00}";
    }

    private void UpdateCoinCount(int count)
    {
        if (coinCountText == null)
        {
            return;
        }

        coinCountText.text = $"Coin {count}";
    }
}

