using UnityEngine;

public sealed class BackGroundScroller : MonoBehaviour
{
    [SerializeField] private Transform[] segments;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private bool moveLeft = true;
    [SerializeField] private bool playOnStart = true;

    private SpriteRenderer[] segmentRenderers;
    private bool isRunning;

    public float Speed => speed;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        CacheRenderers();
        isRunning = playOnStart;
    }

    private void Update()
    {
        if (!isRunning || segments == null || segments.Length == 0)
        {
            return;
        }

        MoveSegments();
        WrapSegments();
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

    private void CacheRenderers()
    {
        if (segments == null)
        {
            segmentRenderers = new SpriteRenderer[0];
            return;
        }

        segmentRenderers = new SpriteRenderer[segments.Length];

        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] != null)
            {
                segmentRenderers[i] = segments[i].GetComponent<SpriteRenderer>();
            }
        }
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

    private void WrapSegments()
    {
        if (targetCamera == null)
        {
            return;
        }

        float leftEdge = GetCameraEdgeX(0f);
        float rightEdge = GetCameraEdgeX(1f);

        for (int i = 0; i < segmentRenderers.Length; i++)
        {
            SpriteRenderer segmentRenderer = segmentRenderers[i];

            if (segmentRenderer == null)
            {
                continue;
            }

            if (moveLeft && segmentRenderer.bounds.max.x < leftEdge)
            {
                MoveSegmentAfterRightMost(i);
            }
            else if (!moveLeft && segmentRenderer.bounds.min.x > rightEdge)
            {
                MoveSegmentBeforeLeftMost(i);
            }
        }
    }

    private float GetCameraEdgeX(float viewportX)
    {
        float distanceFromCamera = Mathf.Abs(targetCamera.transform.position.z - transform.position.z);
        Vector3 edge = targetCamera.ViewportToWorldPoint(new Vector3(viewportX, 0f, distanceFromCamera));
        return edge.x;
    }

    private void MoveSegmentAfterRightMost(int segmentIndex)
    {
        SpriteRenderer segmentRenderer = segmentRenderers[segmentIndex];
        float targetCenterX = GetRightMostEdge() + segmentRenderer.bounds.extents.x;
        MoveSegmentCenterX(segmentIndex, targetCenterX);
    }

    private void MoveSegmentBeforeLeftMost(int segmentIndex)
    {
        SpriteRenderer segmentRenderer = segmentRenderers[segmentIndex];
        float targetCenterX = GetLeftMostEdge() - segmentRenderer.bounds.extents.x;
        MoveSegmentCenterX(segmentIndex, targetCenterX);
    }

    private void MoveSegmentCenterX(int segmentIndex, float targetCenterX)
    {
        float currentCenterX = segmentRenderers[segmentIndex].bounds.center.x;
        float deltaX = targetCenterX - currentCenterX;
        segments[segmentIndex].position += new Vector3(deltaX, 0f, 0f);
    }

    private float GetRightMostEdge()
    {
        float rightMostEdge = float.NegativeInfinity;

        for (int i = 0; i < segmentRenderers.Length; i++)
        {
            if (segmentRenderers[i] != null)
            {
                rightMostEdge = Mathf.Max(rightMostEdge, segmentRenderers[i].bounds.max.x);
            }
        }

        return rightMostEdge;
    }

    private float GetLeftMostEdge()
    {
        float leftMostEdge = float.PositiveInfinity;

        for (int i = 0; i < segmentRenderers.Length; i++)
        {
            if (segmentRenderers[i] != null)
            {
                leftMostEdge = Mathf.Min(leftMostEdge, segmentRenderers[i].bounds.min.x);
            }
        }

        return leftMostEdge;
    }
}
