using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentBlocks", menuName = "Scriptable Objects/EnvironmentBlocks")]
public class EnvironmentBlocks : ScriptableObject
{
    public EnvironmentBlock[] blocks;

    public List<EnvironmentBlock> GetBlocksFromBiome(int biome)
    {
        List<EnvironmentBlock> selectedBlocks = new();
        foreach (EnvironmentBlock block in blocks)
        {
            if (block.biome == biome)
                selectedBlocks.Add(block);
        }
        return selectedBlocks;
    }
}

[Serializable]
public class EnvironmentBlock
{
    public int biome;
    public GameObject prefab;
}
