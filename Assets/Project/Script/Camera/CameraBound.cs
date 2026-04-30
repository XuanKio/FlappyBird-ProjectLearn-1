using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBound : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float gameplayPlaneZ = 0f;

    public float Left {get; private set;}
    public float Right {get; private set;}
    public float Top {get; private set;}
    public float Bottom {get; private set;}

    public float Width => Right - Left;
    public float Height => Top - Bottom;
    public Vector2 Center => new Vector2((Left + Right) / 2f, (Top + Bottom) / 2f);


    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        RefreshBound();
    }

    private void LateUpdate()
    {
        RefreshBound();
    }
    public void RefreshBound()
    {
        float distanceToPlane = Mathf.Abs(targetCamera.transform.position.z - gameplayPlaneZ);

        Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(
            new Vector3(0f, 0f, distanceToPlane)
        );

        Vector3 topRight = targetCamera.ViewportToWorldPoint(
            new Vector3(1f, 1f, distanceToPlane)
        );

        Left = bottomLeft.x;
        Right = topRight.x;
        Bottom = bottomLeft.y;
        Top = topRight.y;
    }
}
