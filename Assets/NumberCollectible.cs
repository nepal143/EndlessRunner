using UnityEngine;

public class NumberCollectible : MonoBehaviour
{
    [Tooltip("How much to increase the number by when the player touches this object")]
    public int increaseAmount = 1;

    [Tooltip("Time before object is destroyed after playing the effect")]
    public float destroyDelay = 1f;

    [Tooltip("Reference to the particle effect GameObject (should be disabled by default and have 'Play On Awake' enabled)")]
    public GameObject particleEffect;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (other.CompareTag("Player"))
        {
            collected = true;

            // Increase the number using CurrentNumberManager
            CurrentNumberManager numberManager = FindObjectOfType<CurrentNumberManager>();
            if (numberManager != null)
            {
                numberManager.IncreaseNumber(increaseAmount);
            }
            else
            {
                Debug.LogWarning("CurrentNumberManager not found in scene!");
            }

            // Enable the particle GameObject
            if (particleEffect != null)
            {
                particleEffect.SetActive(true);
            }

            // Disable visuals & collider
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = false;

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            // Destroy the object after delay
            Destroy(gameObject, destroyDelay);
        }
    }
}
