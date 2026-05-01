using UnityEngine;

public sealed class UnityRandomRange : IRandomRange
{
    public int Range(int minInclusive, int maxExclusive)
    {
        return Random.Range(minInclusive, maxExclusive);
    }

    public float Range(float minInclusive, float maxInclusive)
    {
        return Random.Range(minInclusive, maxInclusive);
    }
}
