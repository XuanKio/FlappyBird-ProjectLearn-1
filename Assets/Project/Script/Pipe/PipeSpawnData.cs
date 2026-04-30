using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct PipeSpawnData
{
    public readonly PipeObstacleType Type;

    public readonly float PassageBottomY;
    public readonly float PassageTopY;

    public readonly float PlayableBottomY;
    public readonly float PlayableTopY;

    public PipeSpawnData(
        PipeObstacleType type,
        float passageBottomY, float passageTopY,
        float playableBottomY, float playableTopY
    )
    {
        Type = type;
        PassageBottomY = passageBottomY;
        PassageTopY = passageTopY;
        PlayableBottomY = playableBottomY;
        PlayableTopY = playableTopY;
    }
    public float PassageHeight => PassageTopY - PassageBottomY;
    public float PassageCenterY => (PassageTopY + PassageBottomY) * 0.5f;
    public float PlayableHeight => PlayableTopY - PlayableBottomY;
}
