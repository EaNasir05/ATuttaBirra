using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameStarted;
    private bool gameOver;
    private bool tutorial;
    public bool policeArrived;
    private int tutorialCheckpoint = 0;

    [Header("Player inputs")]
    [SerializeField] private InputActionAsset actionAsset;
    private InputActionMap actionMap;
    private DeviceType deviceConnected;

    [Header("External scripts")]
    [SerializeField] private CarController carController;
    [SerializeField] private DrinkSystem drinkSystem;
    [SerializeField] private LeverInteraction_InputSystem leverSystem;
    [SerializeField] private LiquidStreamToggle liquidStream;
    [SerializeField] private EntitiesSpawner spawner;
    [SerializeField] private PoliceChaseSystem policeManager;

    [Header("Alcool")]
    [SerializeField] private float maxAlcoolPower;
    [SerializeField] private float minAlcoolPower;
    [SerializeField] private float policeAlcoolPower;
    [SerializeField] private float alcoolPowerConsumedPerSecond;
    [SerializeField] private float ebbrezzaConsumedPerSecond;
    [SerializeField] private float ebbrezzaReductionDelay;
    [SerializeField] private float startingSecondsWithDecelerationImmunity;
    private float secondsWithDecelerationImmunity;
    [SerializeField] private float totalBeerConsumed;
    private float alcoolPower;
    private Coroutine reduceEbbrezzaRoutine;

    [Header("Audios")]
    [SerializeField] private AudioClip accelerationAudioClip;
    [SerializeField] private float accelerationAudioVolume;
    private AudioSource carAudioSource;

    private void Awake()
    {
        instance = this;
        gameStarted = false;
        gameOver = false;
        policeArrived = false;
        actionMap = actionAsset.FindActionMap("Player");
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Cursor.visible = false;
        totalBeerConsumed = 0;
        alcoolPower = 0.25f;
        tutorial = false;
        carController.enabled = false;
        actionMap.FindAction("HoldGlassT").Enable();
        actionMap.FindAction("HoldGlassS").Enable();
        actionMap.FindAction("GrabLeverT").Enable();
        actionMap.FindAction("GrabLeverS").Enable();
        actionMap.FindAction("MoveL").Enable();
        actionMap.FindAction("MoveR").Enable();
        carAudioSource = carController.gameObject.GetComponent<AudioSource>();
        if (StaticGameVariables.instance == null)
            StaticGameVariables.LoadStats();
        HandleDeviceChange(Gamepad.current, InputDeviceChange.Added);
        if (StaticGameVariables.instance.firstTimePlaying)
            tutorial = true;
    }

    private void Start()
    {
        InputSystem.onDeviceChange += HandleDeviceChange;
        if (tutorial)
        {
            StartCoroutine(Tutorial());
            startingSecondsWithDecelerationImmunity = 0;
        }
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= HandleDeviceChange;
    }

    private void OnApplicationQuit()
    {
        StaticGameVariables.SaveStats();
    }

    private void Update()
    {
        if (gameStarted && !UpdateImmunity())
        {
            secondsWithDecelerationImmunity = 0;
            alcoolPower -= alcoolPowerConsumedPerSecond * Time.deltaTime;
            if (alcoolPower < minAlcoolPower && !gameOver)
                StartCoroutine(SpawnPolice());
            if (!gameOver && reduceEbbrezzaRoutine == null)
                reduceEbbrezzaRoutine = StartCoroutine(ReduceEbbrezza());
        }
    }

    private bool UpdateImmunity()
    {
        bool immune = false;
        if (gameStarted && startingSecondsWithDecelerationImmunity > 0)
        {
            startingSecondsWithDecelerationImmunity -= Time.deltaTime;
            immune = true;
        }
        if (gameStarted && secondsWithDecelerationImmunity > 0)
        {
            secondsWithDecelerationImmunity -= Time.deltaTime;
            immune = true;
        }
        return immune;
    }

    public float GetAlcoolPower() => alcoolPower;
    public float GetTotalBeerConsumed() => totalBeerConsumed;
    public bool IsImmuneToDeceleration() => secondsWithDecelerationImmunity > 0;
    public float GetMaxAlcoolPower() => maxAlcoolPower;
    public float GetMinAlcoolPower() => minAlcoolPower;
    public float GetPoliceAlcoolPower() => policeAlcoolPower;

    public void AddDecelerationImmunity(float value)
    {
        SFXManager.instance.PlayClipWithRandomPitch(accelerationAudioClip, accelerationAudioVolume);
        UIManager.instance.StartCameraMovement(value);
        secondsWithDecelerationImmunity += value;
    }

    private void StopReduceEbbrezzaRoutine()
    {
        if (reduceEbbrezzaRoutine != null)
        {
            StopCoroutine(reduceEbbrezzaRoutine);
            reduceEbbrezzaRoutine = null;
        }
    }

    private IEnumerator ReduceEbbrezza()
    {
        yield return new WaitForSeconds(ebbrezzaReductionDelay);
        if (!IsImmuneToDeceleration())
            UIManager.instance.UpdateEbbrezza(-ebbrezzaConsumedPerSecond * ebbrezzaReductionDelay);
        reduceEbbrezzaRoutine = null;
    }

    public void UpdateTotalBeerConsumed(float beerConsumed)
    {
        totalBeerConsumed += beerConsumed;
        UIManager.instance.UpdateBeerConsumed(Mathf.Round(100 * totalBeerConsumed) / 100);
        spawner.UpdateSpawnTime();
        UIManager.instance.CheckBeerPopups(totalBeerConsumed);
    }

    public void UpdateAlcoolPower(float increment)
    {
        if (!gameOver)
        {
            if (!gameStarted)
            {
                if (increment > 0)
                {
                    alcoolPower = 1 + increment;
                    UIManager.instance.StartCameraMovement(increment);
                    StartGame();
                }
            }
            else
            {
                if (increment > 0)
                {
                    AddDecelerationImmunity(4 * increment);
                    StopReduceEbbrezzaRoutine();
                }
                alcoolPower = Mathf.Clamp(alcoolPower + increment, 0, maxAlcoolPower);
            }
        }
    }

    private IEnumerator SpawnPolice()
    {
        gameOver = true;
        actionMap.FindAction("GrabLeverT").Disable();
        actionMap.FindAction("GrabLeverS").Disable();
        actionMap.FindAction("HoldGlassT").Disable();
        actionMap.FindAction("HoldGlassS").Disable();
        actionMap.FindAction("MoveL").Disable();
        actionMap.FindAction("MoveR").Disable();
        StartCoroutine(policeManager.SpawnPoliceCar());
        yield return new WaitUntil(() => policeArrived);
        GameOver();
    }

    private void StartGame()
    {
        if (!tutorial)
            gameStarted = true;
        UIManager.instance.StartGame();
        OSTManager.instance.StartGame();
        carController.enabled = true;
        carController.EnableCarInputs();
        SFXManager.instance.PlayClip(accelerationAudioClip, accelerationAudioVolume);
        carAudioSource.Play();
    }

    private void GameOver()
    {
        gameStarted = false;
        finalScore = totalBeerConsumed;
        UIManager.instance.GameOver();
        StartCoroutine(OSTManager.instance.GameOver());
        alcoolPower = 0;
        carAudioSource.Stop();
    }

    [HideInInspector] public float finalScore;

    private IEnumerator Tutorial()
    {
        drinkSystem.EmptyTheGlass();
        UIManager.instance.EnableHoldLeverTutorialImage(true, deviceConnected);
        yield return new WaitUntil(() => leverSystem.IsGrabbingTheLever());
        StartCoroutine(UIManager.instance.FadeOutTitle());
        tutorialCheckpoint = 1;
        UIManager.instance.EnableHoldLeverTutorialImage(false, deviceConnected);
        UIManager.instance.EnablePullLeverTutorialImage(true, deviceConnected);
        yield return new WaitUntil(() => liquidStream.IsFlowing());
        tutorialCheckpoint = 2;
        UIManager.instance.EnablePullLeverTutorialImage(false, deviceConnected);
        UIManager.instance.EnableHoldGlassTutorialImage(true, deviceConnected);
        yield return new WaitUntil(() => drinkSystem.IsMoving());
        tutorialCheckpoint = 3;
        UIManager.instance.EnableHoldGlassTutorialImage(false, deviceConnected);
        UIManager.instance.EnableMoveGlassTutorialImage(true, deviceConnected);
        yield return new WaitUntil(() => drinkSystem.IsMovingForReal());
        tutorialCheckpoint = 4;
        UIManager.instance.EnableMoveGlassTutorialImage(false, deviceConnected);
        UIManager.instance.EnableFillGlassTutorialImage(true);
        yield return new WaitUntil(() => (drinkSystem.GetBeerFill() < drinkSystem.GetMaxFill() && !liquidStream.IsFillingTheJug()) || drinkSystem.GetBeerFill() <= drinkSystem.GetMinFill());
        tutorialCheckpoint = 5;
        UIManager.instance.EnableFillGlassTutorialImage(false);
        UIManager.instance.EnableDrinkTutorialDirection(true);
        yield return new WaitUntil(() => drinkSystem.IsDrinking());
        tutorialCheckpoint = 6;
        UIManager.instance.EnableDrinkTutorialDirection(false);
        yield return new WaitUntil(() => drinkSystem.IsIdling() || drinkSystem.IsMoving());
        tutorialCheckpoint = 7;
        UIManager.instance.EnableDriveTutorial(true, deviceConnected);
        yield return new WaitUntil(() => carController.GetLastMove() > 0);
        tutorialCheckpoint = 8;
        UIManager.instance.EnableDriveTutorial(false, deviceConnected);
        yield return new WaitForSeconds(2);
        tutorial = false;
        StaticGameVariables.instance.firstTimePlaying = false;
        StartCoroutine(UIManager.instance.FadeInDrinkNDrive());
    }

    private void HandleDeviceChange(InputDevice inputDevice, InputDeviceChange change)
    {
        switch (Gamepad.current)
        {
            case UnityEngine.InputSystem.DualShock.DualShockGamepad:
                deviceConnected = DeviceType.PSController;
                break;
            case UnityEngine.InputSystem.XInput.XInputController:
                deviceConnected = DeviceType.XboxController;
                break;
            case null:
                deviceConnected = DeviceType.KeyboardMouse;
                break;
            default:
                deviceConnected = DeviceType.OtherController;
                break;
        }

        if (tutorial)
        {
            switch (tutorialCheckpoint)
            {
                case 0:
                    UIManager.instance.EnableHoldLeverTutorialImage(true, deviceConnected);
                    break;
                case 1:
                    UIManager.instance.EnablePullLeverTutorialImage(true, deviceConnected);
                    break;
                case 2:
                    UIManager.instance.EnableHoldGlassTutorialImage(true, deviceConnected);
                    break;
                case 3:
                    UIManager.instance.EnableMoveGlassTutorialImage(true, deviceConnected);
                    break;
                case 7:
                    UIManager.instance.EnableDriveTutorial(true, deviceConnected);
                    break;
                default:
                    UnityEngine.Debug.Log("NON SO COSA FARE...");
                    break;
            }
        }
    }
}

public enum DeviceType
{
    KeyboardMouse,
    PSController,
    XboxController,
    OtherController
}
