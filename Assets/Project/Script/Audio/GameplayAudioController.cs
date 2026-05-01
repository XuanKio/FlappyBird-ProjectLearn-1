using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class GameplayAudioController : MonoBehaviour
{
    [SerializeField] private GameplayController gameplayController;
    [SerializeField] private GameplayAudioContainer audioContainer;
    [SerializeField] private AudioSource audioSource;

    private IGameEventBus eventBus;
    private bool isSubscribed;
    private int lastScore;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Start()
    {
        ResolveReferences();
        TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnPlayerHit(PlayerHitEvent eventData)
    {
        PlayOneShot(audioContainer != null ? audioContainer.HitClip : null);
    }

    private void OnPlayerFlapped(PlayerFlappedEvent eventData)
    {
        PlayOneShot(audioContainer != null ? audioContainer.WingClip : null);
    }

    private void OnScoreChanged(ScoreChangedEvent eventData)
    {
        if (eventData.Score > lastScore)
        {
            PlayOneShot(audioContainer != null ? audioContainer.PointClip : null);
        }

        lastScore = eventData.Score;
    }

    private void OnGameStateChanged(GameStateChangedEvent eventData)
    {
        if (eventData.StateId == GameStateId.GameOver)
        {
            PlayOneShot(audioContainer != null ? audioContainer.DieClip : null);
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip == null || audioSource == null)
        {
            return;
        }

        float volume = audioContainer != null ? audioContainer.Volume : 1f;
        audioSource.PlayOneShot(clip, volume);
    }

    private void ResolveReferences()
    {
        if (gameplayController == null)
        {
            gameplayController = FindObjectOfType<GameplayController>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    private void TrySubscribe()
    {
        if (isSubscribed || gameplayController == null || gameplayController.EventBus == null)
        {
            return;
        }

        eventBus = gameplayController.EventBus;
        eventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        eventBus.Subscribe<PlayerHitEvent>(OnPlayerHit);
        eventBus.Subscribe<PlayerFlappedEvent>(OnPlayerFlapped);
        eventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed || eventBus == null)
        {
            return;
        }

        eventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        eventBus.Unsubscribe<PlayerHitEvent>(OnPlayerHit);
        eventBus.Unsubscribe<PlayerFlappedEvent>(OnPlayerFlapped);
        eventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        eventBus = null;
        isSubscribed = false;
    }
}
