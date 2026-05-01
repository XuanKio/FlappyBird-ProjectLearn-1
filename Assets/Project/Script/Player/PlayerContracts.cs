public interface IPlayerMovementController
{
    void EnableMovement();
    void DisableMovement();
    void ResetMovement();
    void SetVisible(bool visible);
}

public interface IPlayerDeathState
{
    void ResetState();
}
