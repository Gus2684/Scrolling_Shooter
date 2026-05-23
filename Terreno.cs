using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Configuración del Terreno")]
    public GameObject[] terrainPrefabs;
    public Transform player;
    public float chunkLength = 30f;
    public int initialChunks = 5;

    private float spawnZ = 0f;
    private Queue<GameObject> activeChunks = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialChunks; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.z - chunkLength > spawnZ - (initialChunks * chunkLength))
        {
            SpawnChunk();
            DeleteOldestChunk();
        }
    }

    void SpawnChunk()
    {
        int randomIndex = Random.Range(0, terrainPrefabs.Length);
        GameObject chunk = Instantiate(terrainPrefabs[randomIndex], Vector3.forward * spawnZ, Quaternion.identity);
        activeChunks.Enqueue(chunk);
        spawnZ += chunkLength;
    }

    void DeleteOldestChunk()
    {
        GameObject oldChunk = activeChunks.Dequeue();
        Destroy(oldChunk);
    }
}
