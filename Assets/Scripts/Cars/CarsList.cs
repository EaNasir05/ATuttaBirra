using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarsList", menuName = "ScriptableObjects/CarsList")]

public class CarsList : ScriptableObject
{
    public Car[] cars;

    public List<Car> GetBigCars()
    {
        List<Car> bigCars = new List<Car>();
        foreach (Car car in cars)
        {
            if (car.IsBig())
                bigCars.Add(car);
        }
        return bigCars;
    }

    public List<Car> GetLittleCars()
    {
        List<Car> littleCars = new List<Car>();
        foreach (Car car in cars)
        {
            if (!car.IsBig())
                littleCars.Add(car);
        }
        return littleCars;
    }
}

[Serializable]
public class Car
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float height;
    [SerializeField] private bool big;

    public GameObject GetPrefab() => prefab;
    public float GetHeight() => height;
    public bool IsBig() => big;
}
