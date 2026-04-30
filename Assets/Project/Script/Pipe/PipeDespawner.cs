using UnityEngine;

public class PipeDespawner : MonoBehaviour
{
    [SerializeField] private float despawnOffsetX = 2f;

    private Camera targetCamera;

    public void Construct(Camera targetCamera)
    {
        this.targetCamera = targetCamera;
    }

    private void Update()
    {
        if (targetCamera == null)
        {
            return;
        }

        float leftEdge = GetCameraLeftEdgeX();

        if (transform.position.x < leftEdge - despawnOffsetX)
        {
            Destroy(gameObject);
        }
    }

    private float GetCameraLeftEdgeX()
    {
        float distance = Mathf.Abs(targetCamera.transform.position.z - transform.position.z);
        Vector3 point = targetCamera.ViewportToWorldPoint(
            new Vector3(0f, 0.5f, distance)
        );

        return point.x;
    }
}
