public sealed class ScoreService : IScoreService
{
    private readonly IGameEventBus eventBus;

    public int CurrentScore { get; private set; }

    public ScoreService(IGameEventBus eventBus)
    {
        this.eventBus = eventBus;
        this.eventBus.Subscribe<PipePassedEvent>(OnPipePassed);
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        eventBus.Publish(new ScoreChangedEvent(CurrentScore));
    }

    private void OnPipePassed(PipePassedEvent eventData)
    {
        CurrentScore++;
        eventBus.Publish(new ScoreChangedEvent(CurrentScore));
    }

    public void Dispose()
    {
        eventBus.Unsubscribe<PipePassedEvent>(OnPipePassed);
    }
}
