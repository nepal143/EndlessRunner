using UnityEngine;

public class NumberCollectible : MonoBehaviour
{
    [Tooltip("How much to increase the number by when the player touches this object")]
    public int increaseAmount = 1;

    [Tooltip("Time before object is destroyed after playing the effect")]
    public float destroyDelay = 1f;

    [Tooltip("Particle effect prefab (should be disabled in the project)")]
    public GameObject particleEffectPrefab;

    [Tooltip("Optional: Transform where the particle effect should spawn")]
    public Transform effectSpawnPoint;

    [Tooltip("Sound to play when collected")]
    public AudioClip collectSound;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        // Increase number
        CurrentNumberManager numberManager = FindObjectOfType<CurrentNumberManager>();
        if (numberManager != null)
        {
            numberManager.IncreaseNumber(increaseAmount);
        }
        else
        {
            Debug.LogWarning("CurrentNumberManager not found in scene!");
        }

        // Determine spawn position (for both sound and particles)
        Vector3 spawnPosition = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;

        // Play collect sound at the spawn point
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, spawnPosition);
        }

        // Spawn particle effect
        if (particleEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(particleEffectPrefab, spawnPosition, Quaternion.identity);
            ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Destroy(effectInstance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effectInstance, 2f); // fallback
            }
        }

        // Hide visuals and disable collider
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Destroy collectible after delay
        Destroy(gameObject, destroyDelay);
    }
}
