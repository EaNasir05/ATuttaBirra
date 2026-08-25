using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [SerializeField] private float spawnPositionZ;
    [SerializeField] private float spawnPositionY;
    [SerializeField] private GameObject[] environmentBlocks;
    [SerializeField] private float blocksSize;
    [SerializeField] private Transform firstBlockLinked;
    private Transform lastBlockSpawned;
    private int spawnIndex = 0;
    private int currentBiome = 0;
    private List<EnvironmentBlock> currentEnvironmentBlocks;

    void Awake()
    {
        lastBlockSpawned = firstBlockLinked;
        //aggiorna currentEnvironmentBlocks
    }

    void Update()
    {
        if (lastBlockSpawned.position.z <= spawnPositionZ - blocksSize)
        {
            lastBlockSpawned = GameObject.Instantiate(environmentBlocks[spawnIndex]).transform;
            lastBlockSpawned.position = new Vector3(0, spawnPositionY, spawnPositionZ);
            spawnIndex++;
            if (spawnIndex == environmentBlocks.Length)
                spawnIndex = 0;
        }
    }

    public void IncreaseBiomeIndex()
    {
        currentBiome++;
        //aggiorna currentEnvironmentBlocks
    }
}
