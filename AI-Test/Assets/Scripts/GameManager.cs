using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damagePerHit = 10;

    public GameState CurrentState { get; private set; } = GameState.Ready;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public int CoinCount { get; private set; }
    public float SurvivalTime { get; private set; }

    public event Action<GameState> OnGameStateChanged;
    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnCoinCountChanged;
    public event Action<float> OnSurvivalTimeChanged;
    public event Action OnGameOver;

    private bool startAfterReload;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Time.timeScale = 1f;

        ResetData();
        SetState(GameState.Ready);
    }

    private void OnEnable()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        SurvivalTime += Time.deltaTime;
        OnSurvivalTimeChanged?.Invoke(SurvivalTime);
    }

    public void StartGame()
    {
        if (CurrentState != GameState.Ready)
        {
            return;
        }

        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        SetState(GameState.Paused);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void TakeDamage(int amount)
    {
        if (CurrentState != GameState.Playing || amount <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            EndGame();
        }
    }

    public void DamagePlayer()
    {
        TakeDamage(damagePerHit);
    }

    public void AddCoin(int amount = 1)
    {
        if (CurrentState != GameState.Playing || amount <= 0)
        {
            return;
        }

        CoinCount += amount;
        OnCoinCountChanged?.Invoke(CoinCount);
    }

    public void ReturnToReady()
    {
        ReloadGame(false);
    }

    public void RestartGame()
    {
        ReloadGame(true);
    }

    private void ReloadGame(bool autoStart)
    {
        startAfterReload = autoStart;

        // 일시정지 상태에서도 씬을 정상적으로 다시 로드합니다.
        Time.timeScale = 1f;

        // 씬 재로드 중에는 게임 진행을 막습니다.
        SetState(GameState.Ready);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetData();
        SetState(GameState.Ready);

        if (startAfterReload)
        {
            startAfterReload = false;
            StartCoroutine(StartAfterSceneReload());
        }
    }

    private IEnumerator StartAfterSceneReload()
    {
        yield return null;
        StartGame();
    }

    private void EndGame()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        SetState(GameState.GameOver);
        Time.timeScale = 0f;

        OnGameOver?.Invoke();
    }

    private void ResetData()
    {
        CurrentHealth = maxHealth;
        CoinCount = 0;
        SurvivalTime = 0f;

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnCoinCountChanged?.Invoke(CoinCount);
        OnSurvivalTimeChanged?.Invoke(SurvivalTime);
    }

    private void SetState(GameState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;
        OnGameStateChanged?.Invoke(CurrentState);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (Instance == this)
        {
            Instance = null;
            Time.timeScale = 1f;
        }
    }
}
