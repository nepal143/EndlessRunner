using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;           // Reference to the player
    public Vector3 offset = new Vector3(0, 5f, -10f); // Adjust based on your scene
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Optional: Look at the player
        // transform.LookAt(target);
    }
}
