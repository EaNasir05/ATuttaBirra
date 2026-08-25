using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarsList", menuName = "Scriptable Objects/CarsList")]

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

    public List<Car> GetBigCars(int biome)
    {
        List<Car> bigCars = new List<Car>();
        foreach (Car car in cars)
        {
            if (car.IsBig() && car.GetBiome() == biome)
                bigCars.Add(car);
        }
        return bigCars;
    }

    public List<Car> GetLittleCars(int biome)
    {
        List<Car> littleCars = new List<Car>();
        foreach (Car car in cars)
        {
            if (!car.IsBig() && car.GetBiome() == biome)
                littleCars.Add(car);
        }
        return littleCars;
    }

    public List<Car> GetCarsFromBiome(int biome)
    {
        List<Car> selectedCars = new List<Car>();
        foreach (Car car in cars)
        {
            if (car.GetBiome() == biome)
                selectedCars.Add(car);
        }
        return selectedCars;
    }
}

[Serializable]
public class Car
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float height;
    [SerializeField] private bool big;
    [SerializeField] private int biome;

    public GameObject GetPrefab() => prefab;
    public float GetHeight() => height;
    public int GetBiome() => biome;
    public bool IsBig() => big;
}
