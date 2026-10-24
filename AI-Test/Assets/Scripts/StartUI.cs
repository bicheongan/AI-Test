using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [Header("Canvas Objects")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    private GameManager subscribedManager;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("StartUI: GameManager가 없습니다.", this);
            return;
        }

        if (startCanvas == null || gameCanvas == null ||
            startButton == null || exitButton == null)
        {
            Debug.LogError("StartUI: Inspector 연결을 확인하세요.", this);
            return;
        }

        subscribedManager = GameManager.Instance;

        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);

        subscribedManager.OnGameStateChanged += HandleGameStateChanged;

        HandleGameStateChanged(subscribedManager.CurrentState);
    }

    private void OnStartButtonClicked()
    {
        subscribedManager.StartGame();
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void HandleGameStateChanged(GameState state)
    {
        bool isReady = state == GameState.Ready;

        startCanvas.SetActive(isReady);
        gameCanvas.SetActive(!isReady);
    }

    private void OnDestroy()
    {
        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartButtonClicked);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClicked);

        if (subscribedManager != null)
            subscribedManager.OnGameStateChanged -= HandleGameStateChanged;
    }
}