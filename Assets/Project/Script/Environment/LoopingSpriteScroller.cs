using UnityEngine;

public sealed class LoopingSpriteScroller : MonoBehaviour
{
    [SerializeField] private Transform[] segments;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float loopStartX = 7.02f;
    [SerializeField] private float loopEndX = -1.76f;
    [SerializeField] private bool moveLeft = true;
    [SerializeField] private bool playOnStart = true;

    private bool isRunning;

    public float Speed => speed;

    private void Awake()
    {
        isRunning = playOnStart && FindObjectOfType<GameplayController>() == null;
    }

    private void Update()
    {
        if (!isRunning || segments == null || segments.Length == 0)
        {
            return;
        }

        MoveSegments();
        LoopSegments();
    }

    public void Play()
    {
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void SetSpeed(float value)
    {
        speed = Mathf.Max(0f, value);
    }

    private void OnValidate()
    {
        speed = Mathf.Max(0f, speed);
    }

    private void MoveSegments()
    {
        Vector3 direction = moveLeft ? Vector3.left : Vector3.right;
        Vector3 movement = direction * speed * Time.deltaTime;

        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] != null)
            {
                segments[i].position += movement;
            }
        }
    }

    private void LoopSegments()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            Transform segment = segments[i];

            if (segment == null)
            {
                continue;
            }

            Vector3 position = segment.position;

            if (moveLeft && position.x <= loopEndX)
            {
                position.x = loopStartX;
                segment.position = position;
            }
            else if (!moveLeft && position.x >= loopStartX)
            {
                position.x = loopEndX;
                segment.position = position;
            }
        }
    }
}
