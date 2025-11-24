using UnityEngine;

public class VerticalCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;     // The player to follow vertically
    [SerializeField] private float yOffset = 2f;    // How much above the player the camera should be
    [SerializeField] private float smoothSpeed = 4f;

    [Header("Horizontal Lock")]
    [SerializeField] private float fixedCameraX = 0f;  // Fixed X position

    private void LateUpdate()
    {
        if (player == null)
            return;

        // Only follow player's Y (height)
        float targetY = player.position.y + yOffset;

        // Smooth vertical movement for nicer feeling
        float newY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * smoothSpeed);

        // Apply position: X is fixed, Z stays the same
        // constant X, smooth Y
        transform.position = new Vector3(fixedCameraX, newY, transform.position.z);
    }
}
