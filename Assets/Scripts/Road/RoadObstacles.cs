using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoadObstacles", menuName = "Scriptable Objects/RoadObstacles")]
public class RoadObstacles : ScriptableObject
{
    public static List<RoadObstacle> SelectObstaclesByLane(List<RoadObstacle> obstacles, RoadLane lane)
    {
        List<RoadObstacle> selectedObstacles = new();
        foreach (RoadObstacle obstacle in obstacles)
        {
            if (obstacle.lane == lane)
                selectedObstacles.Add(obstacle);
        }
        return selectedObstacles;
    }

    public RoadObstacle[] obstacles;

    public List<RoadObstacle> GetObstaclesFromBiome(int biome)
    {
        List<RoadObstacle> selectedObstacles = new();
        foreach (RoadObstacle obstacle in obstacles)
        {
            if (obstacle.biome == biome)
                selectedObstacles.Add(obstacle);
        }
        return selectedObstacles;
    }
}

[Serializable]
public class RoadObstacle
{
    public GameObject prefab;
    public RoadLane lane;
    public int biome;
    public float positionY;
    public float positionZ;
}
