using UnityEngine;
using System.Collections.Generic;

public sealed class LoopingSpriteScroller : MonoBehaviour
{
    [SerializeField] private Transform[] segments;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float wrapPadding = 0.3f;
    [SerializeField] [Range(0f, 1f)] private float wrapTriggerViewportX = 0.4f;
    [SerializeField] private bool moveLeft = true;
    [SerializeField] private bool playOnStart = true;

    private Transform[] resolvedSegments;
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
        if (!isRunning || resolvedSegments == null || resolvedSegments.Length == 0)
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

    private void OnValidate()
    {
        speed = Mathf.Max(0f, speed);
        wrapPadding = Mathf.Max(0f, wrapPadding);
        wrapTriggerViewportX = Mathf.Clamp01(wrapTriggerViewportX);
    }

    private void CacheRenderers()
    {
        if (segments == null)
        {
            resolvedSegments = new Transform[0];
            segmentRenderers = new SpriteRenderer[0];
            return;
        }

        List<Transform> resolvedSegmentList = new List<Transform>();
        List<SpriteRenderer> rendererList = new List<SpriteRenderer>();
        HashSet<Transform> uniqueTransforms = new HashSet<Transform>();

        for (int i = 0; i < segments.Length; i++)
        {
            Transform segment = segments[i];

            if (segment == null)
            {
                continue;
            }

            SpriteRenderer segmentRenderer = segment.GetComponent<SpriteRenderer>();

            if (segmentRenderer != null)
            {
                TryAddResolvedSegment(segment, segmentRenderer, uniqueTransforms, resolvedSegmentList, rendererList);
                continue;
            }

            SpriteRenderer[] childRenderers = segment.GetComponentsInChildren<SpriteRenderer>();

            for (int childIndex = 0; childIndex < childRenderers.Length; childIndex++)
            {
                SpriteRenderer childRenderer = childRenderers[childIndex];
                TryAddResolvedSegment(childRenderer.transform, childRenderer, uniqueTransforms, resolvedSegmentList, rendererList);
            }
        }

        resolvedSegments = resolvedSegmentList.ToArray();
        segmentRenderers = rendererList.ToArray();
    }

    private void MoveSegments()
    {
        Vector3 direction = moveLeft ? Vector3.left : Vector3.right;
        Vector3 movement = direction * speed * Time.deltaTime;

        for (int i = 0; i < resolvedSegments.Length; i++)
        {
            if (resolvedSegments[i] != null)
            {
                resolvedSegments[i].position += movement;
            }
        }
    }

    private void WrapSegments()
    {
        if (targetCamera == null)
        {
            return;
        }

        float triggerX = GetCameraEdgeX(wrapTriggerViewportX);

        for (int i = 0; i < segmentRenderers.Length; i++)
        {
            SpriteRenderer segmentRenderer = segmentRenderers[i];

            if (segmentRenderer == null)
            {
                continue;
            }

            if (moveLeft && segmentRenderer.bounds.max.x < triggerX + wrapPadding)
            {
                MoveSegmentAfterRightMost(i);
            }
            else if (!moveLeft && segmentRenderer.bounds.min.x > triggerX - wrapPadding)
            {
                MoveSegmentBeforeLeftMost(i);
            }
        }
    }

    private float GetCameraEdgeX(float viewportX)
    {
        float distanceFromCamera = Mathf.Abs(targetCamera.transform.position.z - transform.position.z);
        Vector3 edge = targetCamera.ViewportToWorldPoint(new Vector3(viewportX, 0.5f, distanceFromCamera));
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
        resolvedSegments[segmentIndex].position += new Vector3(deltaX, 0f, 0f);
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

    private static void TryAddResolvedSegment(
        Transform segmentTransform,
        SpriteRenderer segmentRenderer,
        HashSet<Transform> uniqueTransforms,
        List<Transform> resolvedSegmentList,
        List<SpriteRenderer> rendererList)
    {
        if (segmentTransform == null || segmentRenderer == null || !uniqueTransforms.Add(segmentTransform))
        {
            return;
        }

        resolvedSegmentList.Add(segmentTransform);
        rendererList.Add(segmentRenderer);
    }
}
