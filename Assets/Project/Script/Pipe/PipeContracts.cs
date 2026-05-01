public interface IPipeCleaner
{
    void ClearAllPipes();
    void StopAllPipes();
}

public interface ISpeedConfigurable
{
    void SetSpeed(float value);
}

public interface IPipeObstacleMetrics
{
    float HalfWidth { get; }
    float UpperHalfHeight { get; }
    float LowerHalfHeight { get; }
}

public interface IPipeSpawnDataFactory
{
    PipeSpawnData Create(PipeObstacleType type);
}

public interface IPipeSpawnPattern
{
    PipeObstacleType Type { get; }
    PipeSpawnData Create(PipeSpawnContext context);
}

public interface IRandomRange
{
    int Range(int minInclusive, int maxExclusive);
    float Range(float minInclusive, float maxInclusive);
}
