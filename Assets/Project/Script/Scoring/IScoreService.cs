using System;

public interface IScoreService : IDisposable
{
    int CurrentScore { get; }
    void ResetScore();
}
