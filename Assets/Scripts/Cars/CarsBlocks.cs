using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[CreateAssetMenu(fileName = "CarsBlocks", menuName = "Scriptable Objects/CarsBlocks")]

public class CarsBlocks : ScriptableObject
{
    public CarsBlock[] blocks;

    public List<CarsBlock> GetCarBlocksFromBiome(int biome)
    {
        List<CarsBlock> selectedBlocks = new();
        foreach(CarsBlock block in blocks)
        {
            if (block.biome == biome)
                selectedBlocks.Add(block);
        }
        return selectedBlocks;
    }
}

[Serializable]
public class CarsBlock
{
    public float[] carsPositionZ;
    public float[] carsPositionX;
    public RoadLane[] carsLane;
    public bool[] bigCars;
    public bool isThereAnObstacle = false;
    public RoadLane obstacleLane;
    public int[] possibleNextBlocksIndexes;
    public int[] possibleBlocksBehindThisObstacle;
    public float spawnDelay = 1.75f;
    public int biome;
}

public enum RoadLane { left, center, right }
