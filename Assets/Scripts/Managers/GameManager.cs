using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Alcool")]
    private float alcoolPower;

    void Awake()
    {
        instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        alcoolPower = 0f;
    }

    void Update()
    {
        
    }

    public float GetAlcoolPower() => alcoolPower;

    public void UpdateAlcoolPower(float beerConsumed)
    {
        alcoolPower += beerConsumed;
        UIManager.instance.UpdateEbrezza();
        UIManager.instance.UpdateBeerConsumed(alcoolPower);
    }
}
