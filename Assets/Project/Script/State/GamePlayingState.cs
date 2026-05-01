using UnityEngine;

public sealed class PlayingState : IGameState
{
    private readonly IPlayerMovementController playerMovement;
    private readonly IPlayerDeathState playerCollision;
    private readonly Transform playerTransform;
    private readonly Vector3 playerStartPosition;

    private readonly IPlayableSystem backgroundScroller;
    private readonly IPlayableSystem pipeSpawner;
    private readonly IPipeCleaner pipeCleaner;
    private readonly IScoreService scoreService;

    public GameStateId StateId => GameStateId.Playing;

    public PlayingState(
        IPlayerMovementController playerMovement,
        IPlayerDeathState playerCollision,
        Transform playerTransform,
        Vector3 playerStartPosition,
        IPlayableSystem backgroundScroller,
        IPlayableSystem pipeSpawner,
        IPipeCleaner pipeCleaner,
        IScoreService scoreService)
    {
        this.playerMovement = playerMovement;
        this.playerCollision = playerCollision;
        this.playerTransform = playerTransform;
        this.playerStartPosition = playerStartPosition;
        this.backgroundScroller = backgroundScroller;
        this.pipeSpawner = pipeSpawner;
        this.pipeCleaner = pipeCleaner;
        this.scoreService = scoreService;
    }

    public void Enter()
    {
        if (pipeCleaner != null)
        {
            pipeCleaner.ClearAllPipes();
        }

        playerTransform.position = playerStartPosition;
        playerMovement.ResetMovement();
        playerCollision.ResetState();

        scoreService.ResetScore();

        playerMovement.SetVisible(true);
        playerMovement.EnableMovement();

        if (backgroundScroller != null)
        {
            backgroundScroller.Play();
        }

        if (pipeSpawner != null)
        {
            pipeSpawner.Play();
        }
    }

    public void Exit()
    {
    }

    public void Tick(float deltaTime)
    {
    }
}
