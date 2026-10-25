using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainTitleButton;
    [SerializeField] private Button resetGameButton;

    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction;

    private GameManager subscribedManager;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("PauseUI: GameManager가 없습니다.", this);
            return;
        }

        if (pausePanel == null ||
            resumeButton == null ||
            mainTitleButton == null ||
            resetGameButton == null ||
            pauseAction == null ||
            pauseAction.action == null)
        {
            Debug.LogError("PauseUI: Inspector 연결을 확인하세요.", this);
            return;
        }

        subscribedManager = GameManager.Instance;

        resumeButton.onClick.AddListener(OnResumeClicked);
        mainTitleButton.onClick.AddListener(OnMainTitleClicked);
        resetGameButton.onClick.AddListener(OnResetGameClicked);

        subscribedManager.OnGameStateChanged += HandleGameStateChanged;

        pauseAction.action.performed += OnPausePerformed;
        pauseAction.action.Enable();

        HandleGameStateChanged(subscribedManager.CurrentState);
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (subscribedManager == null)
        {
            return;
        }

        if (subscribedManager.CurrentState == GameState.Playing)
        {
            subscribedManager.PauseGame();
        }
        else if (subscribedManager.CurrentState == GameState.Paused)
        {
            subscribedManager.ResumeGame();
        }
    }

    private void OnResumeClicked()
    {
        subscribedManager.ResumeGame();
    }

    private void OnMainTitleClicked()
    {
        subscribedManager.ReturnToReady();
    }

    private void OnResetGameClicked()
    {
        subscribedManager.RestartGame();
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(state == GameState.Paused);
        }
    }

    private void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(OnResumeClicked);

        if (mainTitleButton != null)
            mainTitleButton.onClick.RemoveListener(OnMainTitleClicked);

        if (resetGameButton != null)
            resetGameButton.onClick.RemoveListener(OnResetGameClicked);

        if (subscribedManager != null)
            subscribedManager.OnGameStateChanged -= HandleGameStateChanged;

        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPausePerformed;
            pauseAction.action.Disable();
        }
    }
}
