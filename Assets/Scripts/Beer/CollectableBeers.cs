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
    public float fillGlassDurationMultiplier;
    public Color beerLiquidColor;
    public Color beerStreamColor;
    public Color beerFoamColor;
}
