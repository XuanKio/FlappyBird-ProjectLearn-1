public sealed class GameOverState : IGameState
{
    private readonly IGameEventBus eventBus;
    private readonly PlayerMovement playerMovement;
    private readonly LoopingSpriteScroller backgroundScroller;
    private readonly PipeSpawner pipeSpawner;
    private readonly PipeCleaner pipeCleaner;
    private readonly ScoreService scoreService;

    public GameStateId StateId => GameStateId.GameOver;

    public GameOverState(
        IGameEventBus eventBus,
        PlayerMovement playerMovement,
        LoopingSpriteScroller backgroundScroller,
        PipeSpawner pipeSpawner,
        PipeCleaner pipeCleaner,
        ScoreService scoreService)
    {
        this.eventBus = eventBus;
        this.playerMovement = playerMovement;
        this.backgroundScroller = backgroundScroller;
        this.pipeSpawner = pipeSpawner;
        this.pipeCleaner = pipeCleaner;
        this.scoreService = scoreService;
    }

    public void Enter()
    {
        if (playerMovement != null)
        {
            playerMovement.DisableMovement();
        }

        if (backgroundScroller != null)
        {
            backgroundScroller.Stop();
        }

        if (pipeSpawner != null)
        {
            pipeSpawner.Stop();
        }

        if (pipeCleaner != null)
        {
            pipeCleaner.StopAllPipes();
        }

        eventBus.Publish(new GameOverEvent(scoreService.CurrentScore));
    }

    public void Exit()
    {
    }

    public void Tick(float deltaTime)
    {
    }
}
