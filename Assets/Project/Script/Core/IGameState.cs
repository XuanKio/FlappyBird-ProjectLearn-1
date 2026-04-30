public interface IGameState
{
    GameStateId StateId { get; }

    void Enter();
    void Exit();
    void Tick(float deltaTime);
}
