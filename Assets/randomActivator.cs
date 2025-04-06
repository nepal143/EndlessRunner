using UnityEngine;
using System.Collections.Generic;

public class RandomActivator : MonoBehaviour
{
    [Tooltip("Assign exactly 3 GameObjects here")]
    public GameObject[] objectsToChooseFrom;

    private void Start()
    {
        if (objectsToChooseFrom.Length != 3)
        {
            Debug.LogError("Please assign exactly 3 GameObjects to RandomActivator.");
            return;
        }

        // Make sure all are disabled first
        foreach (GameObject obj in objectsToChooseFrom)
        {
            obj.SetActive(false);
        }

        // Randomly choose how many to enable: either 1 or 2
        int countToEnable = Random.Range(1, 3); // will return 1 or 2 

        // Create a shuffled list of indices [0,1,2]
        List<int> indices = new List<int> { 0, 1, 2 };
        for (int i = 0; i < indices.Count; i++)
        {
            int randIndex = Random.Range(i, indices.Count);
            (indices[i], indices[randIndex]) = (indices[randIndex], indices[i]);
        }

        // Enable the first 'countToEnable' GameObjects
        for (int i = 0; i < countToEnable; i++)
        {
            objectsToChooseFrom[indices[i]].SetActive(true);
        }
    }
}
