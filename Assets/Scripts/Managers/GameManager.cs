using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameStarted;

    [Header("Player inputs")]
    [SerializeField] private InputActionAsset actionAsset;
    private InputActionMap actionMap;

    [Header("Alcool")]
    [SerializeField] private float maxAlcoolPower;
    [SerializeField] private float alcoolPowerConsumedPerSecond;
    [SerializeField] private float secondsWithDecelerationImmunity;
    private float totalBeerConsumed;
    private float alcoolPower;
    private float timePassedWithImmunity;

    void Awake()
    {
        instance = this;
        actionMap = actionAsset.FindActionMap("Player");
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        totalBeerConsumed = 0;
        alcoolPower = 1;
        timePassedWithImmunity = 0;
        gameStarted = true;
    }

    void Update()
    {
        if (gameStarted && timePassedWithImmunity < secondsWithDecelerationImmunity)
            timePassedWithImmunity += Time.deltaTime;
        else if (gameStarted)
        {
            alcoolPower -= alcoolPowerConsumedPerSecond * Time.deltaTime;
            if (alcoolPower < 0.5f)
                GameOver();
        }
    }

    public float GetAlcoolPower() => alcoolPower;
    public float GetTotalBeerConsumed() => totalBeerConsumed;

    public void UpdateTotalBeerConsumed(float beerConsumed)
    {
        totalBeerConsumed += beerConsumed;
        UIManager.instance.UpdateBeerConsumed(Mathf.Round(totalBeerConsumed * 100) / 100);
        UIManager.instance.UpdateEbrezza();
    }

    public void UpdateAlcoolPower(float beerConsumed)
    {
        if (alcoolPower + beerConsumed >= maxAlcoolPower)
            alcoolPower = maxAlcoolPower;
        else
            alcoolPower += beerConsumed;
    }

    private void GameOver()
    {
        gameStarted = false;
        UIManager.instance.GameOver();
        alcoolPower = 0;
        actionMap.FindAction("Hold T").Disable();
        actionMap.FindAction("Hold S").Disable();
        actionMap.FindAction("Move").Disable();
        actionMap.FindAction("Speed").Disable();
        Debug.Log("GAME OVER");
    }
}
