using System.Collections.Generic;

public sealed class GameStateMachine
{
    private readonly Dictionary<GameStateId, IGameState> states = new();
    private readonly IGameEventBus eventBus;

    private IGameState currentState;

    public GameStateId CurrentStateId => currentState != null
        ? currentState.StateId
        : GameStateId.Ready;

    public GameStateMachine(IGameEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public void RegisterState(IGameState state)
    {
        if (state == null)
        {
            return;
        }

        states[state.StateId] = state;
    }

    public void ChangeState(GameStateId stateId)
    {
        if (currentState != null && currentState.StateId == stateId)
        {
            return;
        }

        if (!states.TryGetValue(stateId, out IGameState nextState))
        {
            return;
        }

        currentState?.Exit();

        currentState = nextState;
        currentState.Enter();

        eventBus.Publish(new GameStateChangedEvent(stateId));
    }

    public void Tick(float deltaTime)
    {
        currentState?.Tick(deltaTime);
    }
}
