using UnityEngine;

public sealed class GameplayUIController : MonoBehaviour
{
    [SerializeField] private GameplayController gameplayController;
    [SerializeField] private GameObject gameStartPrompt;
    [SerializeField] private GameObject gameplayScoreRoot;
    [SerializeField] private SpriteNumberDisplay gameplayScoreDisplay;
    [SerializeField] private GameObject gameOverRoot;
    [SerializeField] private SpriteNumberDisplay finalScoreDisplay;

    private IGameEventBus eventBus;
    private bool isSubscribed;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Start()
    {
        ResolveReferences();
        TrySubscribe();
        RefreshState(gameplayController != null ? gameplayController.CurrentStateId : GameStateId.Ready);
        SetScore(gameplayController != null ? gameplayController.CurrentScore : 0, false);
    }

    private void Update()
    {
        if (gameplayController == null ||
            gameplayController.CurrentStateId != GameStateId.Ready ||
            !Input.GetMouseButtonDown(0))
        {
            return;
        }

        gameplayController.StartGame();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnGameStateChanged(GameStateChangedEvent eventData)
    {
        RefreshState(eventData.StateId);
    }

    private void OnScoreChanged(ScoreChangedEvent eventData)
    {
        SetScore(eventData.Score, true);
    }

    private void OnGameOver(GameOverEvent eventData)
    {
        if (finalScoreDisplay != null)
        {
            finalScoreDisplay.SetValue(eventData.FinalScore, true);
        }
    }

    private void RefreshState(GameStateId stateId)
    {
        SetActive(gameStartPrompt, stateId == GameStateId.Ready);
        SetActive(gameplayScoreRoot, stateId == GameStateId.Playing);
        SetActive(gameOverRoot, stateId == GameStateId.GameOver);

        if (stateId == GameStateId.Ready)
        {
            SetScore(0, false);
        }
        else if (stateId == GameStateId.GameOver && finalScoreDisplay != null)
        {
            finalScoreDisplay.SetValue(gameplayController != null ? gameplayController.CurrentScore : 0, false);
        }
    }

    private void SetScore(int score, bool animate)
    {
        if (gameplayScoreDisplay != null)
        {
            gameplayScoreDisplay.SetValue(score, animate);
        }
    }

    private void ResolveReferences()
    {
        if (gameplayController == null)
        {
            gameplayController = FindObjectOfType<GameplayController>();
        }

        if (gameStartPrompt == null)
        {
            gameStartPrompt = GameObject.Find("GameStart");
        }

        if (gameplayScoreRoot == null)
        {
            gameplayScoreRoot = GameObject.Find("GameScore");
        }

        if (gameplayScoreDisplay == null && gameplayScoreRoot != null)
        {
            gameplayScoreDisplay = gameplayScoreRoot.GetComponent<SpriteNumberDisplay>();
        }

        if (gameOverRoot == null)
        {
            gameOverRoot = GameObject.Find("GameOver");
        }

        if (finalScoreDisplay == null && gameOverRoot != null)
        {
            Transform scoreTransform = gameOverRoot.transform.Find("Score");

            if (scoreTransform != null)
            {
                finalScoreDisplay = scoreTransform.GetComponent<SpriteNumberDisplay>();
            }
        }
    }

    private void TrySubscribe()
    {
        if (isSubscribed || gameplayController == null || gameplayController.EventBus == null)
        {
            return;
        }

        eventBus = gameplayController.EventBus;
        eventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        eventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        eventBus.Subscribe<GameOverEvent>(OnGameOver);
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed || eventBus == null)
        {
            return;
        }

        eventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        eventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        eventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        eventBus = null;
        isSubscribed = false;
    }

    private static void SetActive(GameObject target, bool active)
    {
        if (target != null && target.activeSelf != active)
        {
            target.SetActive(active);
        }
    }
}
