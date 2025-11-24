using UnityEngine;

public class CollectibleSuperJump : MonoBehaviour
{
    [SerializeField] private string triggeringTag = "Player";
    [SerializeField] private float powerDuration = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(triggeringTag)) return;

        Mover mover = other.GetComponent<Mover>();
        if (mover != null)
        {
            mover.ActivateSuperJump(powerDuration);
        }

        gameObject.SetActive(false);
    }
}
