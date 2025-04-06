using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject tilePrefab;
    public float tileLength = 10f;
    public int tilesAhead = 5;
    public int maxTiles = 7;

    private float spawnZ = 0f;
    private Queue<GameObject> spawnedTiles = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < tilesAhead; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        if (player.position.z + (tilesAhead * tileLength) > spawnZ)
        {
            SpawnTile();
            if (spawnedTiles.Count > maxTiles)
            {
                GameObject oldTile = spawnedTiles.Dequeue();
                Destroy(oldTile);
            }
        }
    }

    void SpawnTile()
    {
        GameObject tile = Instantiate(tilePrefab, new Vector3(0, 0, spawnZ), Quaternion.identity);
        spawnedTiles.Enqueue(tile);
        spawnZ += tileLength;
    }
}
