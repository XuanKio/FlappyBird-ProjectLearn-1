using UnityEngine;

public class PipeMovement : MonoBehaviour, IPlayableSystem, ISpeedConfigurable
{
    [SerializeField] private float pipeSpeed = 3f;
    private bool isMoving = false;

    private void Update()
    {
        if (!isMoving) return;
        transform.position += Vector3.left * pipeSpeed * Time.deltaTime;
    }

    public void Play()
    {
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }

    public void SetSpeed(float value)
    {
        pipeSpeed = Mathf.Max(0f, value);
    }
}
