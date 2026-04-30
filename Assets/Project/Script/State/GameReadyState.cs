public sealed class ReadyState : IGameState
{
    private readonly PlayerMovement playerMovement;
    private readonly LoopingSpriteScroller backgroundScroller;
    private readonly PipeSpawner pipeSpawner;

    public GameStateId StateId => GameStateId.Ready;

    public ReadyState(
        PlayerMovement playerMovement,
        LoopingSpriteScroller backgroundScroller,
        PipeSpawner pipeSpawner)
    {
        this.playerMovement = playerMovement;
        this.backgroundScroller = backgroundScroller;
        this.pipeSpawner = pipeSpawner;
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
    }

    public void Exit()
    {
    }

    public void Tick(float deltaTime)
    {
    }
}
