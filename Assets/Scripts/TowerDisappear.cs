using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDisappear : MonoBehaviour
{
    [Header("Bricks List (Bottom to Top)")]
    [SerializeField] private List<GameObject> bricks = new List<GameObject>();

    [Header("Timing")]
    [SerializeField] private float startDelay = 15f;   // Time before the first brick starts to fall
    [SerializeField] private float interval = 3f;      // Time between each brick

    [Header("Falling Settings")]
    [SerializeField] private float fallDuration = 2f;      // How long the brick falls before disappearing
    [SerializeField] private float fallGravityScale = 3f;  // Gravity scale while falling

    private void Start()
    {
        StartCoroutine(RemoveBricksRoutine());
    }

    private IEnumerator RemoveBricksRoutine()
    {
        // Wait before starting the decay
        yield return new WaitForSeconds(startDelay);

        // Iterate bricks from bottom to top
        foreach (GameObject brick in bricks)
        {
            if (brick != null)
            {
                // Make the brick fall
                StartFalling(brick);

                // Wait while it falls
                yield return new WaitForSeconds(fallDuration);

                // After falling time - disable the brick
                brick.SetActive(false);
            }

            // Wait before the next brick starts falling
            yield return new WaitForSeconds(interval);
        }
    }

    private void StartFalling(GameObject brick)
    {
        // Get or add Rigidbody2D
        Rigidbody2D rb = brick.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = brick.AddComponent<Rigidbody2D>();
        }

        // Set it to Dynamic so it will fall with physics
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravityScale;

        // Optional: remove any platform effector so the brick will behave like a normal falling object
        PlatformEffector2D effector = brick.GetComponent<PlatformEffector2D>();
        if (effector != null)
        {
            Destroy(effector);
        }
    }
}
