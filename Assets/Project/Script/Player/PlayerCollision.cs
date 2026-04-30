using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class PlayerCollision : MonoBehaviour
{
    private IGameEventBus eventBus;
    private bool isDead;

    public void Construct(IGameEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    public void ResetState()
    {
        isDead = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ShouldDieFrom(collision.collider))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ShouldDieFrom(other))
        {
            Die();
        }
    }

    private bool ShouldDieFrom(Component other)
    {
        return !isDead &&
               (other.CompareTag(GameTags.Obstacle) ||
                other.CompareTag(GameTags.Ground));
    }

    private void Die()
    {
        isDead = true;
        eventBus?.Publish(new PlayerDiedEvent());
    }
}
