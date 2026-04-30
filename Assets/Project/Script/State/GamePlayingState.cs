using UnityEngine;

public sealed class PlayingState : IGameState
{
    private readonly PlayerMovement playerMovement;
    private readonly PlayerCollision playerCollision;
    private readonly Transform playerTransform;
    private readonly Vector3 playerStartPosition;

    private readonly LoopingSpriteScroller backgroundScroller;
    private readonly PipeSpawner pipeSpawner;
    private readonly PipeCleaner pipeCleaner;
    private readonly ScoreService scoreService;

    public GameStateId StateId => GameStateId.Playing;

    public PlayingState(
        PlayerMovement playerMovement,
        PlayerCollision playerCollision,
        Transform playerTransform,
        Vector3 playerStartPosition,
        LoopingSpriteScroller backgroundScroller,
        PipeSpawner pipeSpawner,
        PipeCleaner pipeCleaner,
        ScoreService scoreService)
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
