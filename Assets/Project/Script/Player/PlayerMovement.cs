using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float jumpVelocity = 5f;
    [SerializeField] private float maxFallSpeed = -8f;

    [Header("Smoothing")]
    [SerializeField] private bool interpolateMovement = true;

    [Header("Rotation")]
    [SerializeField] private bool rotateByVelocity = true;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float fallRotationSpeed = 18f;
    [SerializeField] private float upRotation = 25f;
    [SerializeField] private float downRotation = -70f;

    private IGameplayInputReader inputReader;
    private bool canMove;
    private bool jumpRequested;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        ApplyRigidbodySettings();
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        ApplyRigidbodySettings();
    }

    public void Construct(IGameplayInputReader inputReader)
    {
        this.inputReader = inputReader;
    }

    public void EnableMovement()
    {
        canMove = true;
        jumpRequested = true;

        rb.simulated = true;
        rb.velocity = Vector2.zero;

        transform.rotation = Quaternion.identity;
    }

    public void DisableMovement()
    {
        canMove = false;
        jumpRequested = false;

        rb.velocity = Vector2.zero;
        rb.simulated = false;
    }

    public void ResetMovement()
    {
        jumpRequested = false;
        rb.velocity = Vector2.zero;
        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        if (!canMove)
        {
            return;
        }

        ReadInput();

        if (rotateByVelocity)
        {
            UpdateRotation();
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }

        ApplyJumpIfRequested();
        ClampFallSpeed();
    }

    private void ReadInput()
    {
        if (inputReader != null && inputReader.JumpPressed)
        {
            jumpRequested = true;
        }
    }

    private void ApplyJumpIfRequested()
    {
        if (!jumpRequested)
        {
            return;
        }

        jumpRequested = false;

        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
    }

    private void ClampFallSpeed()
    {
        if (rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
    }

    private void UpdateRotation()
    {
        bool isFalling = rb.velocity.y <= 0f;
        float targetAngle = isFalling ? downRotation : upRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        float currentRotationSpeed = isFalling ? fallRotationSpeed : rotationSpeed;
        float smoothStep = 1f - Mathf.Exp(-currentRotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            smoothStep
        );
    }

    private void ApplyRigidbodySettings()
    {
        if (rb == null)
        {
            return;
        }

        rb.interpolation = interpolateMovement
            ? RigidbodyInterpolation2D.Interpolate
            : RigidbodyInterpolation2D.None;
    }

    private void OnValidate()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        jumpVelocity = Mathf.Max(0f, jumpVelocity);
        maxFallSpeed = Mathf.Min(0f, maxFallSpeed);
        rotationSpeed = Mathf.Max(0f, rotationSpeed);
        fallRotationSpeed = Mathf.Max(0f, fallRotationSpeed);

        ApplyRigidbodySettings();
    }
}
