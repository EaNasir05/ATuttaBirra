using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameStarted;

    [Header("Player inputs")]
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private CarController carController;
    private InputActionMap actionMap;

    [Header("Alcool")]
    [SerializeField] private float maxAlcoolPower;
    [SerializeField] private float alcoolPowerConsumedPerSecond;
    [SerializeField] private float secondsWithDecelerationImmunity;
    private float totalBeerConsumed;
    private float alcoolPower;

    void Awake()
    {
        instance = this;
        gameStarted = false;
        actionMap = actionAsset.FindActionMap("Player");
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        totalBeerConsumed = 0;
        alcoolPower = 0.25f;
        carController.enabled = false;
        actionMap.FindAction("Hold T").Enable();
        actionMap.FindAction("Hold S").Enable();
        actionMap.FindAction("Move").Enable();
        actionMap.FindAction("Speed").Enable();
    }

    void Update()
    {
        if (gameStarted && secondsWithDecelerationImmunity > 0)
            secondsWithDecelerationImmunity -= Time.deltaTime;
        else if (gameStarted)
        {
            secondsWithDecelerationImmunity = 0;
            alcoolPower -= alcoolPowerConsumedPerSecond * Time.deltaTime;
            if (alcoolPower < 0.5f)
                GameOver();
        }
    }

    public float GetAlcoolPower() => alcoolPower;
    public float GetTotalBeerConsumed() => totalBeerConsumed;
    public bool IsImmuneToDeceleration() => secondsWithDecelerationImmunity > 0;

    public void AddDecelerationImmunity(float value)
    {
        secondsWithDecelerationImmunity += value;
    }

    public void UpdateTotalBeerConsumed(float beerConsumed)
    {
        totalBeerConsumed += beerConsumed;
        UIManager.instance.UpdateBeerConsumed(Mathf.Round(totalBeerConsumed * 100) / 100);
        UIManager.instance.UpdateEbrezza();
    }

    public void UpdateAlcoolPower(float increment)
    {
        if (!gameStarted)
        {
            alcoolPower = 0.5f + increment;
            StartGame();
        }
        else
        {
            Debug.Log(increment);
            if (Mathf.Sign(increment) > 0)
                secondsWithDecelerationImmunity += increment * 4;
            alcoolPower = Mathf.Clamp(alcoolPower + increment, 0, maxAlcoolPower);
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        UIManager.instance.StartGame();
        carController.enabled = true;
        Debug.Log("GAME STARTED");
    }

    private void GameOver()
    {
        gameStarted = false;
        UIManager.instance.GameOver();
        alcoolPower = 0;
        actionMap.FindAction("Grab").Disable();
        actionMap.FindAction("Hold T").Disable();
        actionMap.FindAction("Hold S").Disable();
        actionMap.FindAction("Move").Disable();
        actionMap.FindAction("Speed").Disable();
        Debug.Log("GAME OVER");
    }
}
