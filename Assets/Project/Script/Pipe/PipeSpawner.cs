using UnityEngine;

public class PipeSpawner : MonoBehaviour, IPlayableSystem
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private PipeObstacle pipePrefab;

    [Header("Spawn Types")]
    [SerializeField] private PipeObstacleType[] spawnTypes =
    {
        PipeObstacleType.FullPair,
        PipeObstacleType.FullPair,
        PipeObstacleType.LowerOnly,
        PipeObstacleType.UpperOnly
    };

    [Header("Spawn")]
    [SerializeField] private float spawnOffsetX = 1.5f;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private bool playOnStart = true;

    [Header("Full Pair Gap")]
    [SerializeField] private float minGapSize = 2.3f;
    [SerializeField] private float maxGapSize = 3.2f;

    [Header("Single Pipe Pattern")]
    [SerializeField] private float minSingleOpenHeight = 3f;
    [SerializeField] private float minSinglePipeHeight = 1.5f;

    [Header("World Bounds")]
    [SerializeField] private float upperMaxY = 7.9f;
    [SerializeField] private float lowerMinY = -6.53f;
    [SerializeField] private float upperPipeMinY = 4.64f;
    [SerializeField] private float upperPipeMaxY = 7.45f;
    [SerializeField] private float lowerPipeMinY = -6.66f;
    [SerializeField] private float lowerPipeMaxY = -3.11f;

    [Header("Movement")]
    [SerializeField] private float pipeSpeed = 3f;

    private float timer;
    private bool isSpawning;
    private IGameEventBus eventBus;
    private IRandomRange randomRange;
    private IPipeSpawnDataFactory spawnDataFactory;

    public void Construct(IGameEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        randomRange = new UnityRandomRange();
        RebuildSpawnDataFactory();
    }

    private void Start()
    {
        if (playOnStart && FindObjectOfType<GameplayController>() == null)
        {
            Play();
        }
    }

    private void Update()
    {
        if (!isSpawning)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    public void Play()
    {
        isSpawning = true;
        timer = spawnInterval;
    }

    public void Stop()
    {
        isSpawning = false;
    }

    public void SetPipeSpeed(float value)
    {
        pipeSpeed = Mathf.Max(0f, value);
    }

    private void Spawn()
    {
        if (pipePrefab == null)
        {
            Debug.LogError("PipeSpawner is missing a pipe prefab reference.", this);
            Stop();
            return;
        }

        PipeObstacleType type = GetRandomType();
        RebuildSpawnDataFactory();

        PipeSpawnData spawnData = spawnDataFactory.Create(type);

        float spawnX = GetCameraRightEdgeX() + pipePrefab.HalfWidth + spawnOffsetX;

        Vector3 spawnPosition = new Vector3(spawnX, 0f, 0f);

        PipeObstacle pipe = Instantiate(pipePrefab, spawnPosition, Quaternion.identity);

        pipe.Construct(eventBus);
        pipe.Setup(spawnData);

        PipeMovement movement = pipe.GetComponent<PipeMovement>();

        if (movement != null)
        {
            movement.SetSpeed(pipeSpeed);
            movement.Play();
        }

        PipeDespawner despawner = pipe.GetComponent<PipeDespawner>();

        if (despawner != null)
        {
            despawner.Construct(targetCamera);
        }
    }

    private PipeObstacleType GetRandomType()
    {
        if (spawnTypes == null || spawnTypes.Length == 0)
        {
            return PipeObstacleType.FullPair;
        }

        randomRange ??= new UnityRandomRange();

        int index = randomRange.Range(0, spawnTypes.Length);
        return spawnTypes[index];
    }

    private void RebuildSpawnDataFactory()
    {
        if (pipePrefab == null)
        {
            spawnDataFactory = null;
            return;
        }

        randomRange ??= new UnityRandomRange();
        spawnDataFactory = PipeSpawnDataFactory.CreateDefault(
            CreateSpawnRules(),
            pipePrefab,
            randomRange
        );
    }

    private PipeSpawnRules CreateSpawnRules()
    {
        return new PipeSpawnRules(
            minGapSize,
            maxGapSize,
            minSingleOpenHeight,
            minSinglePipeHeight,
            upperMaxY,
            lowerMinY,
            upperPipeMinY,
            upperPipeMaxY,
            lowerPipeMinY,
            lowerPipeMaxY
        );
    }

    private float GetCameraRightEdgeX()
    {
        float distance = Mathf.Abs(targetCamera.transform.position.z - transform.position.z);
        Vector3 point = targetCamera.ViewportToWorldPoint(
            new Vector3(1f, 0.5f, distance)
        );

        return point.x;
    }

    private void OnValidate()
    {
        spawnOffsetX = Mathf.Max(0f, spawnOffsetX);
        spawnInterval = Mathf.Max(0.1f, spawnInterval);
        minGapSize = Mathf.Max(0.1f, minGapSize);
        maxGapSize = Mathf.Max(minGapSize, maxGapSize);
        minSingleOpenHeight = Mathf.Max(0.1f, minSingleOpenHeight);
        minSinglePipeHeight = Mathf.Max(0.1f, minSinglePipeHeight);

        if (upperMaxY < lowerMinY)
        {
            float temp = upperMaxY;
            upperMaxY = lowerMinY;
            lowerMinY = temp;
        }

        if (upperPipeMaxY < upperPipeMinY)
        {
            float temp = upperPipeMaxY;
            upperPipeMaxY = upperPipeMinY;
            upperPipeMinY = temp;
        }

        upperPipeMinY = Mathf.Clamp(upperPipeMinY, lowerMinY, upperMaxY);
        upperPipeMaxY = Mathf.Clamp(upperPipeMaxY, lowerMinY, upperMaxY);

        if (lowerPipeMaxY < lowerPipeMinY)
        {
            float temp = lowerPipeMaxY;
            lowerPipeMaxY = lowerPipeMinY;
            lowerPipeMinY = temp;
        }

        pipeSpeed = Mathf.Max(0f, pipeSpeed);
    }
}
