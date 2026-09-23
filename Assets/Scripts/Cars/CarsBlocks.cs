using System;
using System.Collections.Generic;
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
    public CarDisposition[] cars;
    public bool isThereAnObstacle = false;
    public RoadObstacle obstacle;
    public int[] possibleNextBlocksIndexes;
    public float spawnDelay = 1.75f;
    public int biome;
}

[Serializable]
public class CarDisposition
{
    public float positionZ;
    public float positionX;
    public RoadLane lane;
    public bool big;
    public bool mirage;
}

[Serializable]
public class RoadObstacle
{
    public GameObject prefab;
    public RoadLane lane;
    public float positionY;
    public float positionZ;
    public int[] possibleBlocksBehindThisObstacle;
}

public enum RoadLane { left, center, right, extendedLeft, extendedRight }
