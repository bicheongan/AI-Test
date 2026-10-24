
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text coinCountText;

    [Header("Health UI")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBarFill;

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
        subscribedManager.OnHealthChanged += UpdateHealth;

        // 현재 데이터를 UI에 즉시 반영
        UpdateSurvivalTime(subscribedManager.SurvivalTime);
        UpdateCoinCount(subscribedManager.CoinCount);

        UpdateHealth(
            subscribedManager.CurrentHealth,
            subscribedManager.MaxHealth
        );
    }

    private void OnDisable()
    {
        if (subscribedManager == null)
        {
            return;
        }

        subscribedManager.OnSurvivalTimeChanged -= UpdateSurvivalTime;
        subscribedManager.OnCoinCountChanged -= UpdateCoinCount;
        subscribedManager.OnHealthChanged -= UpdateHealth;

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

    private void UpdateHealth(int currentHealth, int maxHealth)
    {
        // 기존 체력 텍스트 갱신
        if (healthText != null)
        {
            healthText.text =
                $"{maxHealth}/{currentHealth}";
        }

        // 빨간 체력 게이지 갱신
        if (healthBarFill != null)
        {
            float healthRatio = maxHealth > 0
                ? (float) currentHealth / maxHealth
                : 0f;

            healthBarFill.fillAmount =
                Mathf.Clamp01(healthRatio);
        }
    }
}
