using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject tilePrefab;
    public float tileLength = 10f;
    public int tilesAhead = 8;
    public int maxTiles = 15;

    private float spawnZ = 92f; // Start spawning from z = 92
    private Queue<GameObject> spawnedTiles = new Queue<GameObject>();

    void Start()
    {
        // Initial spawning of tiles from z = 92 up to the initial tilesAhead
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
