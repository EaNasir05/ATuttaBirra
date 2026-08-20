using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableBeers", menuName = "Scriptable Objects/CollectableBeers")]

public class CollectableBeers : ScriptableObject
{
    public CollectableBeer[] beers;
}

[Serializable]
public class CollectableBeer
{
    public string beerName;
    public float ebbrezzaMultiplier;
    public float speedBoostMultiplier;
    public float immunityDurationMultiplier;
    public float drinkDurationMultiplier;
    public float beerStreamEfficiencyMultiplier;
    public Material beerMaterial;
    public ParticleSystem beerStreamParticles;
    public Color beerSplashParticleColor;
    public GameObject beerOverflowObject;
    public bool unlocked;
}
