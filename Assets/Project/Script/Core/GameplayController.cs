using UnityEngine;

public sealed class GameplayController : MonoBehaviour, IGameplayReadModel, IGameplayCommands
{
    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCollision playerCollision;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 playerStartPosition;

    [Header("Gameplay")]
    [SerializeField] private LoopingSpriteScroller backgroundScroller;
    [SerializeField] private PipeSpawner pipeSpawner;
    [SerializeField] private PipeCleaner pipeCleaner;

    [Header("Input")]
    [SerializeField] private MonoBehaviour inputReaderBehaviour;

    private IGameplayInputReader inputReader;
    private IGameEventBus eventBus;
    private GameStateMachine stateMachine;
    private IScoreService scoreService;

    public IGameEventBus EventBus => eventBus;
    public GameStateId CurrentStateId => stateMachine != null
        ? stateMachine.CurrentStateId
        : GameStateId.Ready;
    public int CurrentScore => scoreService != null
        ? scoreService.CurrentScore
        : 0;

    private void Awake()
    {
        ResolveReferences();

        if (!HasRequiredReferences())
        {
            enabled = false;
            return;
        }

        eventBus = new GameEventBus();
        scoreService = new ScoreService(eventBus);

        inputReader = ResolveInputReader();

        if (inputReader == null)
        {
            Debug.LogError("Input Reader Behaviour must implement IGameplayInputReader.");
            enabled = false;
            return;
        }

        playerMovement.Construct(inputReader, eventBus);
        playerMovement.SetVisible(false);
        playerCollision.Construct(eventBus);
        pipeSpawner.Construct(eventBus);

        stateMachine = new GameStateMachine(eventBus);

        stateMachine.RegisterState(new ReadyState(
            playerMovement,
            backgroundScroller,
            pipeSpawner
        ));

        stateMachine.RegisterState(new PlayingState(
            playerMovement,
            playerCollision,
            playerTransform,
            playerStartPosition,
            backgroundScroller,
            pipeSpawner,
            pipeCleaner,
            scoreService
        ));

        stateMachine.RegisterState(new GameOverState(
            eventBus,
            playerMovement,
            backgroundScroller,
            pipeSpawner,
            pipeCleaner,
            scoreService
        ));

        eventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDestroy()
    {
        if (eventBus != null)
        {
            eventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        scoreService?.Dispose();
    }

    private void Start()
    {
        if (stateMachine == null)
        {
            return;
        }

        stateMachine.ChangeState(GameStateId.Ready);
    }

    private void Update()
    {
        if (stateMachine == null)
        {
            return;
        }

        float deltaTime = Time.deltaTime;

        HandleInput();
        stateMachine.Tick(deltaTime);
    }

    private void HandleInput()
    {
        if (inputReader == null)
        {
            return;
        }

        if (!inputReader.JumpPressed)
        {
            return;
        }

        switch (stateMachine.CurrentStateId)
        {
            case GameStateId.Ready:
                StartGame();
                break;

            case GameStateId.GameOver:
                PlayAgain();
                break;
        }
    }

    public void StartGame()
    {
        stateMachine.ChangeState(GameStateId.Playing);
    }

    public void PlayAgain()
    {
        stateMachine.ChangeState(GameStateId.Playing);
    }

    private void OnPlayerDied(PlayerDiedEvent eventData)
    {
        if (stateMachine.CurrentStateId != GameStateId.Playing)
        {
            return;
        }

        stateMachine.ChangeState(GameStateId.GameOver);
    }

    private void ResolveReferences()
    {
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
        }

        if (playerTransform == null && playerMovement != null)
        {
            playerTransform = playerMovement.transform;
        }

        if (playerStartPosition == Vector3.zero && playerTransform != null)
        {
            playerStartPosition = playerTransform.position;
        }

        if (playerCollision == null && playerMovement != null)
        {
            playerCollision = playerMovement.GetComponent<PlayerCollision>();

            if (playerCollision == null)
            {
                playerCollision = playerMovement.gameObject.AddComponent<PlayerCollision>();
            }
        }

        if (backgroundScroller == null)
        {
            backgroundScroller = FindObjectOfType<LoopingSpriteScroller>();
        }

        if (pipeSpawner == null)
        {
            pipeSpawner = FindObjectOfType<PipeSpawner>();
        }

        if (pipeCleaner == null)
        {
            pipeCleaner = FindObjectOfType<PipeCleaner>();
        }
    }

    private bool HasRequiredReferences()
    {
        if (playerMovement != null &&
            playerCollision != null &&
            playerTransform != null)
        {
            return true;
        }

        Debug.LogError("GameplayController is missing required player references.", this);
        return false;
    }

    private IGameplayInputReader ResolveInputReader()
    {
        IGameplayInputReader resolvedInputReader = GameplayInputReaderResolver.Resolve(
            inputReaderBehaviour,
            playerMovement,
            out MonoBehaviour resolvedBehaviour
        );

        inputReaderBehaviour = resolvedBehaviour;
        return resolvedInputReader;
    }
}
