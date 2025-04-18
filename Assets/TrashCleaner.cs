using UnityEngine;

public class TrashCleaner : MonoBehaviour
{
    public GameObject exception1;
    public GameObject exception2;

    private float timer = 0f;
    private float duration = 90f;

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;

            // Find all objects tagged "Trash"
            GameObject[] trashObjects = GameObject.FindGameObjectsWithTag("Trash");

            foreach (GameObject trash in trashObjects)
            {
                // Skip the two exceptions
                if (trash != exception1 && trash != exception2)
                {
                    Destroy(trash);
                }
            }
        }
    }
}
