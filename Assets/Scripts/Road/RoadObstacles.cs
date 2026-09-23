using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoadObstacles", menuName = "Scriptable Objects/RoadObstacles")]
public class RoadObstacles : ScriptableObject
{
    public RoadObstacle[] obstacles;

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
}
