public sealed class GameOverState : IGameState
{
    private readonly IGameEventBus eventBus;
    private readonly IPlayerMovementController playerMovement;
    private readonly IPlayableSystem backgroundScroller;
    private readonly IPlayableSystem pipeSpawner;
    private readonly IPipeCleaner pipeCleaner;
    private readonly IScoreService scoreService;

    public GameStateId StateId => GameStateId.GameOver;

    public GameOverState(
        IGameEventBus eventBus,
        IPlayerMovementController playerMovement,
        IPlayableSystem backgroundScroller,
        IPlayableSystem pipeSpawner,
        IPipeCleaner pipeCleaner,
        IScoreService scoreService)
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
            playerMovement.SetVisible(true);
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
