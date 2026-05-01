using UnityEngine;

public class PlayerInstaller : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MonoBehaviour inputReaderBehaviour;
    [SerializeField] private bool enableMovementOnAwake = true;

    private void Awake()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        IGameplayInputReader inputReader = ResolveInputReader();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement reference is missing.");
            return;
        }

        if (inputReader == null)
        {
            Debug.LogError("Input Reader Behaviour must implement IGameplayInputReader.");
            return;
        }

        playerMovement.Construct(inputReader);

        if (enableMovementOnAwake && FindObjectOfType<GameplayController>() == null)
        {
            playerMovement.EnableMovement();
        }
    }

    private IGameplayInputReader ResolveInputReader()
    {
        IGameplayInputReader resolvedInputReader = GameplayInputReaderResolver.Resolve(
            inputReaderBehaviour,
            this,
            out MonoBehaviour resolvedBehaviour
        );

        inputReaderBehaviour = resolvedBehaviour;
        return resolvedInputReader;
    }
}
