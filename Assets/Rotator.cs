using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0); // Default Y-axis rotation
    public float rotationSpeed = 100f; // Degrees per second
    public bool randomizeStartRotation = true;
    public bool pingPong = false; // Optional back-and-forth rotation (like floating text style)

    private float direction = 1f;

    void Start()
    {
        if (randomizeStartRotation)
        {
            transform.rotation = Random.rotation;
        }
    }

    void Update()
    {
        if (pingPong)
        {
            // Optional ping-pong logic (reverses every second for variation)
            direction = Mathf.Sin(Time.time * 2f); 
        }

        transform.Rotate(rotationAxis * rotationSpeed * direction * Time.deltaTime, Space.Self);
    }
}
