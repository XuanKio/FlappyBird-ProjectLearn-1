public sealed class ReadyState : IGameState
{
    private readonly IPlayerMovementController playerMovement;
    private readonly IPlayableSystem backgroundScroller;
    private readonly IPlayableSystem pipeSpawner;

    public GameStateId StateId => GameStateId.Ready;

    public ReadyState(
        IPlayerMovementController playerMovement,
        IPlayableSystem backgroundScroller,
        IPlayableSystem pipeSpawner)
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
            playerMovement.SetVisible(false);
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
