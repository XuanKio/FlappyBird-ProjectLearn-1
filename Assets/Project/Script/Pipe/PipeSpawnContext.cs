public readonly struct PipeSpawnContext
{
    public readonly PipeSpawnRules Rules;
    public readonly IPipeObstacleMetrics PipeMetrics;
    public readonly IRandomRange RandomRange;

    public PipeSpawnContext(
        PipeSpawnRules rules,
        IPipeObstacleMetrics pipeMetrics,
        IRandomRange randomRange)
    {
        Rules = rules;
        PipeMetrics = pipeMetrics;
        RandomRange = randomRange;
    }

    public float PlayableTopY => Rules.UpperMaxY;
    public float PlayableBottomY => Rules.LowerMinY;

    public float SafeRandom(float min, float max)
    {
        if (min >= max)
        {
            return (min + max) * 0.5f;
        }

        return RandomRange.Range(min, max);
    }
}
