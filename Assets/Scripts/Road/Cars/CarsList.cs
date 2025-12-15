using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CarsList", menuName = "ScriptableObjects/CarsList")]

public class CarsList : ScriptableObject
{
    public Car[] cars;
}

[Serializable]
public class Car
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float height;

    public GameObject GetPrefab() => prefab;
    public float GetHeight() => height;
}
