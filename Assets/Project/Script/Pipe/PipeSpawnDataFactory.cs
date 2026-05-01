using System.Collections.Generic;
using UnityEngine;

public sealed class PipeSpawnDataFactory : IPipeSpawnDataFactory
{
    private readonly Dictionary<PipeObstacleType, IPipeSpawnPattern> patterns = new();
    private readonly PipeSpawnContext context;
    private readonly PipeObstacleType fallbackType;

    public PipeSpawnDataFactory(
        PipeSpawnContext context,
        IEnumerable<IPipeSpawnPattern> spawnPatterns,
        PipeObstacleType fallbackType = PipeObstacleType.FullPair)
    {
        this.context = context;
        this.fallbackType = fallbackType;

        foreach (IPipeSpawnPattern pattern in spawnPatterns)
        {
            if (pattern != null)
            {
                patterns[pattern.Type] = pattern;
            }
        }
    }

    public static PipeSpawnDataFactory CreateDefault(
        PipeSpawnRules rules,
        IPipeObstacleMetrics pipeMetrics,
        IRandomRange randomRange)
    {
        return new PipeSpawnDataFactory(
            new PipeSpawnContext(rules, pipeMetrics, randomRange),
            new IPipeSpawnPattern[]
            {
                new FullPairPattern(),
                new LowerOnlyPattern(),
                new UpperOnlyPattern()
            }
        );
    }

    public PipeSpawnData Create(PipeObstacleType type)
    {
        if (patterns.TryGetValue(type, out IPipeSpawnPattern pattern))
        {
            return pattern.Create(context);
        }

        return patterns[fallbackType].Create(context);
    }

    private sealed class FullPairPattern : IPipeSpawnPattern
    {
        public PipeObstacleType Type => PipeObstacleType.FullPair;

        public PipeSpawnData Create(PipeSpawnContext context)
        {
            PipeSpawnRules rules = context.Rules;
            float gapSize = context.SafeRandom(rules.MinGapSize, rules.MaxGapSize);
            float lowerPipeY = context.SafeRandom(rules.LowerPipeMinY, rules.LowerPipeMaxY);
            float passageBottomY = lowerPipeY + context.PipeMetrics.LowerHalfHeight;
            float passageTopY = passageBottomY + gapSize;

            return new PipeSpawnData(
                Type,
                passageBottomY,
                passageTopY,
                context.PlayableBottomY,
                context.PlayableTopY
            );
        }
    }

    private sealed class LowerOnlyPattern : IPipeSpawnPattern
    {
        public PipeObstacleType Type => PipeObstacleType.LowerOnly;

        public PipeSpawnData Create(PipeSpawnContext context)
        {
            PipeSpawnRules rules = context.Rules;
            float lowerHalfHeight = context.PipeMetrics.LowerHalfHeight;
            float minLowerPipeY = Mathf.Max(
                rules.LowerPipeMinY,
                context.PlayableBottomY + rules.MinSinglePipeHeight - lowerHalfHeight
            );

            float maxLowerPipeY = Mathf.Min(
                rules.LowerPipeMaxY,
                context.PlayableTopY - rules.MinSingleOpenHeight - lowerHalfHeight
            );

            float lowerPipeY = context.SafeRandom(minLowerPipeY, maxLowerPipeY);
            lowerPipeY = Mathf.Clamp(lowerPipeY, rules.LowerPipeMinY, rules.LowerPipeMaxY);
            float lowerTopY = lowerPipeY + lowerHalfHeight;

            return new PipeSpawnData(
                Type,
                lowerTopY,
                context.PlayableTopY,
                context.PlayableBottomY,
                context.PlayableTopY
            );
        }
    }

    private sealed class UpperOnlyPattern : IPipeSpawnPattern
    {
        public PipeObstacleType Type => PipeObstacleType.UpperOnly;

        public PipeSpawnData Create(PipeSpawnContext context)
        {
            PipeSpawnRules rules = context.Rules;
            float upperPipeY = context.SafeRandom(rules.UpperPipeMinY, rules.UpperPipeMaxY);
            float upperBottomY = Mathf.Max(
                context.PlayableBottomY + rules.MinSingleOpenHeight,
                upperPipeY - context.PipeMetrics.UpperHalfHeight
            );

            return new PipeSpawnData(
                Type,
                context.PlayableBottomY,
                upperBottomY,
                context.PlayableBottomY,
                context.PlayableTopY
            );
        }
    }
}
