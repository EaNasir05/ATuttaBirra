using UnityEngine;
using System.IO;

public class StaticGameVariables
{
    public static StaticGameVariables instance;
    public float record;
    public float totalBeerConsumed;
    public bool firstTimePlaying;
    public int maxBiomeReached;
    public bool[] unlockedBeers;
    public bool[] unlockedGlasses;

    public StaticGameVariables()
    {
        record = 0;
        totalBeerConsumed = 0;
        firstTimePlaying = true;
        maxBiomeReached = 0;
        unlockedBeers = new bool[10];
        unlockedGlasses = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            unlockedBeers[i] = false;
            unlockedGlasses[i] = false;
        }
    }

    public static void SaveStats()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        string data = JsonUtility.ToJson(instance);
        File.WriteAllText(path, data);
    }

    public static void LoadStats()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            instance = JsonUtility.FromJson<StaticGameVariables>(data);
        }
        else
        {
            instance = new StaticGameVariables();
        }
    }
}
