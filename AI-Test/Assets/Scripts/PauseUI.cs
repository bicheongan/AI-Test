using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button ResetButton;

    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction;

    private GameManager subscribedManager;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "PauseUI: GameManager를 찾을 수 없습니다.",
                this
            );
            return;
        }

        if (pausePanel == null ||
            resumeButton == null ||
            pauseAction == null ||
            pauseAction.action == null)
        {
            Debug.LogError(
                "PauseUI: Inspector 연결을 확인하세요.",
                this
            );
            return;
        }

        subscribedManager = GameManager.Instance;

        resumeButton.onClick.AddListener(OnResumeClicked);

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
        if (subscribedManager != null)
        {
            subscribedManager.ResumeGame();
        }
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
        {
            resumeButton.onClick.RemoveListener(OnResumeClicked);
        }

        if (subscribedManager != null)
        {
            subscribedManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPausePerformed;
            pauseAction.action.Disable();
        }
    }
}
