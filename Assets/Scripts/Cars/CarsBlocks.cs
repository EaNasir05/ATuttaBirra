using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CarsBlocks", menuName = "Scriptable Objects/CarsBlocks")]

public class CarsBlocks : ScriptableObject
{
    public CarsBlock[] blocks;
}

[Serializable]
public class CarsBlock
{
    public float[] carsPositionZ;
    public float[] carsPositionX;
    public RoadLane[] carsLane;
    public bool[] bigCars;
    public int[] possibleNextBlocksIndexes;
}

public enum RoadLane { left, center, right }
