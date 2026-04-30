using UnityEngine;

public sealed class PipeCleaner : MonoBehaviour
{
    public void ClearAllPipes()
    {
        PipeObstacle[] pipes = FindObjectsOfType<PipeObstacle>();

        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }
    }

    public void StopAllPipes()
    {
        PipeMovement[] pipeMovements = FindObjectsOfType<PipeMovement>();

        for (int i = 0; i < pipeMovements.Length; i++)
        {
            pipeMovements[i].Stop();
        }
    }
}