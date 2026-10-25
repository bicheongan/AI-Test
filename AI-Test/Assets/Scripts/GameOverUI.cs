using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Result Text")]
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text finalCoinText;

    [Header("Buttons")]
    [SerializeField] private Button resetGameButton;
    [SerializeField] private Button mainTitleButton;

    private GameManager subscribedManager;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameOverUI: GameManager가 없습니다.", this);
            return;
        }

        if (gameOverPanel == null ||
            finalTimeText == null ||
            finalCoinText == null ||
            resetGameButton == null ||
            mainTitleButton == null)
        {
            Debug.LogError("GameOverUI: Inspector 연결을 확인하세요.", this);
            return;
        }

        subscribedManager = GameManager.Instance;

        resetGameButton.onClick.AddListener(OnResetGameClicked);
        mainTitleButton.onClick.AddListener(OnMainTitleClicked);

        subscribedManager.OnGameStateChanged += HandleGameStateChanged;

        HandleGameStateChanged(subscribedManager.CurrentState);
    }

    private void HandleGameStateChanged(GameState state)
    {
        bool isGameOver = state == GameState.GameOver;

        gameOverPanel.SetActive(isGameOver);

        if (isGameOver)
        {
            UpdateResult();
        }
    }

    private void UpdateResult()
    {
        float survivalTime = subscribedManager.SurvivalTime;
        int coinCount = subscribedManager.CoinCount;

        int totalSeconds = Mathf.FloorToInt(survivalTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        finalTimeText.text =
            $"{minutes:00}:{seconds:00}";

        finalCoinText.text =
            $"{coinCount}";
    }

    private void OnResetGameClicked()
    {
        subscribedManager.RestartGame();
    }

    private void OnMainTitleClicked()
    {
        subscribedManager.ReturnToReady();
    }

    private void OnDestroy()
    {
        if (resetGameButton != null)
            resetGameButton.onClick.RemoveListener(OnResetGameClicked);

        if (mainTitleButton != null)
            mainTitleButton.onClick.RemoveListener(OnMainTitleClicked);

        if (subscribedManager != null)
            subscribedManager.OnGameStateChanged -= HandleGameStateChanged;
    }
}
