public readonly struct PipeSpawnRules
{
    public readonly float MinGapSize;
    public readonly float MaxGapSize;
    public readonly float MinSingleOpenHeight;
    public readonly float MinSinglePipeHeight;
    public readonly float UpperMaxY;
    public readonly float LowerMinY;
    public readonly float UpperPipeMinY;
    public readonly float UpperPipeMaxY;
    public readonly float LowerPipeMinY;
    public readonly float LowerPipeMaxY;

    public PipeSpawnRules(
        float minGapSize,
        float maxGapSize,
        float minSingleOpenHeight,
        float minSinglePipeHeight,
        float upperMaxY,
        float lowerMinY,
        float upperPipeMinY,
        float upperPipeMaxY,
        float lowerPipeMinY,
        float lowerPipeMaxY)
    {
        MinGapSize = minGapSize;
        MaxGapSize = maxGapSize;
        MinSingleOpenHeight = minSingleOpenHeight;
        MinSinglePipeHeight = minSinglePipeHeight;
        UpperMaxY = upperMaxY;
        LowerMinY = lowerMinY;
        UpperPipeMinY = upperPipeMinY;
        UpperPipeMaxY = upperPipeMaxY;
        LowerPipeMinY = lowerPipeMinY;
        LowerPipeMaxY = lowerPipeMaxY;
    }
}
