using UnityEngine;

public class KeyboardGameplayInputReader : MonoBehaviour, IGameplayInputReader
{
    public bool JumpPressed => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetMouseButtonDown(0);
}
