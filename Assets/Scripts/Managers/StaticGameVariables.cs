using UnityEngine;

public class StaticGameVariables
{
    public static StaticGameVariables instance;
    public float record;
    public float totalBeerConsumed;
    public bool firstTimePlaying;

    public StaticGameVariables()
    {
        record = 0;
        totalBeerConsumed = 0;
        firstTimePlaying = true;
    }
}
