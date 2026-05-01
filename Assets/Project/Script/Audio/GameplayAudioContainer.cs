using UnityEngine;

[CreateAssetMenu(
    fileName = "GameplayAudioContainer",
    menuName = "Flappy Bird/Audio/Gameplay Audio Container")]
public sealed class GameplayAudioContainer : ScriptableObject
{
    [Header("Gameplay")]
    [SerializeField] private AudioClip pointClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip wingClip;
    [SerializeField] private AudioClip dieClip;

    [Header("Mix")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    public AudioClip PointClip => pointClip;
    public AudioClip HitClip => hitClip;
    public AudioClip WingClip => wingClip;
    public AudioClip DieClip => dieClip;
    public float Volume => volume;
}
