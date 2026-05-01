using UnityEngine;

public class PipeObstacle : MonoBehaviour, IPipeObstacleMetrics
{
    [Header("Upper Pipe")]
    [SerializeField] private GameObject upperPipeRoot;
    [SerializeField] private Transform upperPipe;
    [SerializeField] private SpriteRenderer upperRenderer;
    [SerializeField] private BoxCollider2D upperCollider;

    [Header("Lower Pipe")]
    [SerializeField] private GameObject lowerPipeRoot;
    [SerializeField] private Transform lowerPipe;
    [SerializeField] private SpriteRenderer lowerRenderer;
    [SerializeField] private BoxCollider2D lowerCollider;

    [Header("Win Zone")]
    [SerializeField] private BoxCollider2D winZone;

    [Header("Size")]
    [SerializeField] private float pipeWidth = 1f;
    [SerializeField] private float winZoneWidth = 1f;

    public PipeObstacleType Type { get; private set; }
    public float Width => GetPipeWidth();
    public float HalfWidth => Width * 0.5f;
    public float UpperHalfHeight => GetPipeHalfHeight(upperRenderer, upperCollider);
    public float LowerHalfHeight => GetPipeHalfHeight(lowerRenderer, lowerCollider);

    public void Construct(IGameEventBus eventBus)
    {
        WinZone winZoneComponent = GetWinZoneComponent();

        if (winZoneComponent != null)
        {
            winZoneComponent.Construct(eventBus);
        }
    }

    private void Reset()
    {
        ResolveReferences();
    }

    private void Awake()
    {
        ResolveReferences();
    }

    public void SetUp(PipeSpawnData spawnData)
    {
        Setup(spawnData);
    }

    public void Setup(PipeSpawnData spawnData)
    {
        ResolveReferences();

        Type = spawnData.Type;

        bool hasLower = spawnData.Type == PipeObstacleType.FullPair ||
                        spawnData.Type == PipeObstacleType.LowerOnly;

        bool hasUpper = spawnData.Type == PipeObstacleType.FullPair ||
                        spawnData.Type == PipeObstacleType.UpperOnly;

        SetUpperActive(hasUpper);
        SetLowerActive(hasLower);

        if (hasLower)
        {
            SetupLowerPipe(spawnData.PassageBottomY);
        }

        if (hasUpper)
        {
            SetupUpperPipe(spawnData.PassageTopY);
        }

        float winBottomY = hasLower ? spawnData.PassageBottomY : spawnData.PlayableBottomY;
        float winTopY = hasUpper ? spawnData.PassageTopY : spawnData.PlayableTopY;

        SetupWinZone(winBottomY, winTopY);
    }

    private void SetUpperActive(bool active)
    {
        if (upperPipeRoot != null)
        {
            upperPipeRoot.SetActive(active);
        }
    }

    private void SetLowerActive(bool active)
    {
        if (lowerPipeRoot != null)
        {
            lowerPipeRoot.SetActive(active);
        }
    }

    private void SetupLowerPipe(float lowerTopY)
    {
        if (lowerPipe == null)
        {
            return;
        }

        float lowerCenterY = lowerTopY - GetPipeHalfHeight(lowerRenderer, lowerCollider);

        Vector3 localPosition = lowerPipe.localPosition;
        localPosition.x = 0f;
        localPosition.y = lowerCenterY - transform.position.y;
        lowerPipe.localPosition = localPosition;

        SetSimpleDrawMode(lowerRenderer);
    }

    private void SetupUpperPipe(float upperBottomY)
    {
        if (upperPipe == null)
        {
            return;
        }

        float upperCenterY = upperBottomY + GetPipeHalfHeight(upperRenderer, upperCollider);

        Vector3 localPosition = upperPipe.localPosition;
        localPosition.x = 0f;
        localPosition.y = upperCenterY - transform.position.y;
        upperPipe.localPosition = localPosition;

        SetSimpleDrawMode(upperRenderer);
    }

    private static void SetSimpleDrawMode(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.drawMode = SpriteDrawMode.Simple;
        }
    }

    private void SetupWinZone(float passageBottomY, float passageTopY)
    {
        if (winZone == null)
        {
            return;
        }

        float height = Mathf.Max(0.1f, passageTopY - passageBottomY);
        float centerY = (passageTopY + passageBottomY) * 0.5f;

        winZone.isTrigger = true;

        Vector3 localPosition = winZone.transform.localPosition;
        localPosition.x = 0f;
        localPosition.y = centerY - transform.position.y;
        winZone.transform.localPosition = localPosition;

        winZone.size = new Vector2(winZoneWidth, height);
        winZone.offset = Vector2.zero;
    }

    private void ResolveReferences()
    {
        ResolvePipeReferences();

        if (upperPipe != null)
        {
            upperPipeRoot ??= upperPipe.gameObject;
            upperRenderer ??= upperPipe.GetComponent<SpriteRenderer>();
            upperCollider ??= upperPipe.GetComponent<BoxCollider2D>();
        }

        if (lowerPipe != null)
        {
            lowerPipeRoot ??= lowerPipe.gameObject;
            lowerRenderer ??= lowerPipe.GetComponent<SpriteRenderer>();
            lowerCollider ??= lowerPipe.GetComponent<BoxCollider2D>();
        }

        if (winZone == null)
        {
            Transform winZoneTransform = transform.Find("ScoreZone");

            if (winZoneTransform != null)
            {
                winZone = winZoneTransform.GetComponent<BoxCollider2D>();
            }
        }
    }

    private void ResolvePipeReferences()
    {
        if (upperPipe != null && lowerPipe != null)
        {
            return;
        }

        Transform upperNamedPipe = transform.Find("UpperPipe");
        Transform lowerNamedPipe = transform.Find("LowerPipe");

        if (upperNamedPipe == null || lowerNamedPipe == null)
        {
            return;
        }

        Transform topPipe = upperNamedPipe.localPosition.y >= lowerNamedPipe.localPosition.y
            ? upperNamedPipe
            : lowerNamedPipe;

        Transform bottomPipe = topPipe == upperNamedPipe ? lowerNamedPipe : upperNamedPipe;

        upperPipe ??= topPipe;
        lowerPipe ??= bottomPipe;
    }

    private float GetPipeWidth()
    {
        float width = 0f;

        width = Mathf.Max(width, GetPipeSize(upperRenderer, upperCollider).x);
        width = Mathf.Max(width, GetPipeSize(lowerRenderer, lowerCollider).x);

        return width > 0f ? width : pipeWidth;
    }

    private static float GetPipeHalfHeight(SpriteRenderer spriteRenderer, BoxCollider2D boxCollider)
    {
        Vector2 size = GetPipeSize(spriteRenderer, boxCollider);
        return Mathf.Max(0.05f, size.y * 0.5f);
    }

    private static Vector2 GetPipeSize(SpriteRenderer spriteRenderer, BoxCollider2D boxCollider)
    {
        if (spriteRenderer != null)
        {
            return spriteRenderer.bounds.size;
        }

        if (boxCollider != null)
        {
            return boxCollider.bounds.size;
        }

        return Vector2.zero;
    }

    private void OnValidate()
    {
        pipeWidth = Mathf.Max(0.1f, pipeWidth);
        winZoneWidth = Mathf.Max(0.1f, winZoneWidth);
    }

    private WinZone GetWinZoneComponent()
    {
        ResolveReferences();

        if (winZone == null)
        {
            return GetComponentInChildren<WinZone>();
        }

        WinZone winZoneComponent = winZone.GetComponent<WinZone>();

        if (winZoneComponent == null)
        {
            winZoneComponent = winZone.gameObject.AddComponent<WinZone>();
        }

        return winZoneComponent;
    }
}
