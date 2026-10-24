using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private Canvas startCanvas;
    [SerializeField] private Canvas gameCanvas;

    [Header("Button References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    private GameManager subscribedManager;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "StartUI: GameManager를 찾을 수 없습니다.",
                this
            );
            return;
        }

        if (startCanvas == null ||
            gameCanvas == null ||
            startButton == null ||
            exitButton == null)
        {
            Debug.LogError(
                "StartUI: Canvas 또는 Button 연결을 확인하세요.",
                this
            );
            return;
        }

        subscribedManager = GameManager.Instance;

        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);

        subscribedManager.OnGameStateChanged += HandleGameStateChanged;

        // 게임 시작 시 현재 상태에 맞게 Canvas 표시
        HandleGameStateChanged(subscribedManager.CurrentState);
    }

    private void OnStartButtonClicked()
    {
        if (subscribedManager == null)
        {
            return;
        }

        subscribedManager.StartGame();
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Game Over");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void HandleGameStateChanged(GameState state)
    {
        bool isReady = state == GameState.Ready;

        // 시작 화면
        startCanvas.enabled = isReady;

        // 실제 게임 HUD
        gameCanvas.enabled = !isReady;
    }

    private void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        if (subscribedManager != null)
        {
            subscribedManager.OnGameStateChanged -= HandleGameStateChanged;
        }
    }
}
