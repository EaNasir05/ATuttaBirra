using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Alcool")]
    private float totalBeerConsumed;
    private float alcoolPower;

    void Awake()
    {
        instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        totalBeerConsumed = 0;
        alcoolPower = 0f;
    }

    void Update()
    {
        
    }

    public float GetAlcoolPower() => alcoolPower;
    public float GetTotalBeerConsumed() => totalBeerConsumed;

    public void UpdateTotalBeerConsumed(float beerConsumed)
    {
        totalBeerConsumed += beerConsumed;
        UIManager.instance.UpdateBeerConsumed(Mathf.Round(totalBeerConsumed * 100) / 100);
    }

    public void UpdateAlcoolPower(float beerConsumed)
    {
        alcoolPower += beerConsumed;
        UIManager.instance.UpdateEbrezza();
    }
}
