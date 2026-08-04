using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableGlasses", menuName = "Scriptable Objects/CollectableGlasses")]

public class CollectableGlasses : ScriptableObject
{
    public CollectableGlass[] glasses;
}

[Serializable]
public class CollectableGlass
{
    public string glassName;
    public GameObject glassPrefab;
    public bool unlocked;
}
