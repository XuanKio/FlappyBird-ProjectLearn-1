public readonly struct PlayerDiedEvent
{
}

public readonly struct PipePassedEvent
{
}

public readonly struct ScoreChangedEvent
{
    public readonly int Score;

    public ScoreChangedEvent(int score)
    {
        Score = score;
    }
}

public readonly struct GameOverEvent
{
    public readonly int FinalScore;

    public GameOverEvent(int finalScore)
    {
        FinalScore = finalScore;
    }
}

public readonly struct GameStateChangedEvent
{
    public readonly GameStateId StateId;

    public GameStateChangedEvent(GameStateId stateId)
    {
        StateId = stateId;
    }
}
