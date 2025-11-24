using UnityEngine;

public class CameraWidthFit : MonoBehaviour
{
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;

    private void Start()
    {
        Camera cam = GetComponent<Camera>();

        float distance = Mathf.Abs(rightWall.position.x - leftWall.position.x);

        // Set orthographic size so camera shows exactly wall-to-wall
        cam.orthographicSize = distance / (2f * cam.aspect);
    }
}
