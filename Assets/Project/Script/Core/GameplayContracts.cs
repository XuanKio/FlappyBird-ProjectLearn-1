public interface IGameplayReadModel
{
    IGameEventBus EventBus { get; }
    GameStateId CurrentStateId { get; }
    int CurrentScore { get; }
}

public interface IGameplayCommands
{
    void StartGame();
    void PlayAgain();
}

public interface IPlayableSystem
{
    void Play();
    void Stop();
}
