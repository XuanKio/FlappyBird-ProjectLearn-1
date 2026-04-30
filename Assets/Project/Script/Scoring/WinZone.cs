using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class WinZone : MonoBehaviour
{
    [SerializeField] private bool scoreOnce = true;

    private IGameEventBus eventBus;
    private bool hasScored;

    public void Construct(IGameEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public void ResetState()
    {
        hasScored = false;
    }

    private void Awake()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (scoreOnce && hasScored)
        {
            return;
        }

        if (!other.CompareTag(GameTags.Player))
        {
            return;
        }

        hasScored = true;
        eventBus?.Publish(new PipePassedEvent());
    }
}
