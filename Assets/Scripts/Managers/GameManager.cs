using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameStarted;
    public bool policeArrived;

    [Header("Player inputs")]
    [SerializeField] private InputActionAsset actionAsset;
    private InputActionMap actionMap;

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
    [SerializeField] private float startingSecondsWithDecelerationImmunity;
    private float secondsWithDecelerationImmunity;
    private float totalBeerConsumed;
    private float alcoolPower;
    private bool gameOver;
    private bool tutorial;

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
        actionMap.FindAction("Hold T").Enable();
        actionMap.FindAction("Hold S").Enable();
        actionMap.FindAction("Move").Enable();
        actionMap.FindAction("Speed").Enable();
        carAudioSource = carController.gameObject.GetComponent<AudioSource>();
        StaticGameVariables.instance ??= new();
        if (StaticGameVariables.instance.firstTimePlaying)
            tutorial = true;
    }

    private void Start()
    {
        if (tutorial)
        {
            StartCoroutine(Tutorial());
            startingSecondsWithDecelerationImmunity = 0;
        }
    }

    private void Update()
    {
        if (gameStarted && !UpdateImmunity())
        {
            secondsWithDecelerationImmunity = 0;
            alcoolPower -= alcoolPowerConsumedPerSecond * Time.deltaTime;
            if (alcoolPower < minAlcoolPower && !gameOver)
                StartCoroutine(SpawnPolice());
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

    public void UpdateTotalBeerConsumed(float beerConsumed)
    {
        totalBeerConsumed += beerConsumed;
        UIManager.instance.UpdateBeerConsumed(Mathf.Round(totalBeerConsumed * 100) / 100);
        UIManager.instance.UpdateEbrezza();
        spawner.UpdateSpawnTime();
        UIManager.instance.CheckBeerPopups(totalBeerConsumed);
    }

    public void UpdateAlcoolPower(float increment)
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
                AddDecelerationImmunity(increment * 4);
            alcoolPower = Mathf.Clamp(alcoolPower + increment, 0, maxAlcoolPower);
        }
    }

    private IEnumerator SpawnPolice()
    {
        gameOver = true;
        actionMap.FindAction("Grab").Disable();
        actionMap.FindAction("Hold T").Disable();
        actionMap.FindAction("Hold S").Disable();
        actionMap.FindAction("Move").Disable();
        actionMap.FindAction("Speed").Disable();
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
        UIManager.instance.EnableHoldLeverTutorialImage(true);
        yield return new WaitUntil(() => leverSystem.IsGrabbingTheLever());
        StartCoroutine(UIManager.instance.FadeOutTitle());
        UIManager.instance.EnableHoldLeverTutorialImage(false);
        UIManager.instance.EnablePullLeverTutorialImage(true);
        yield return new WaitUntil(() => liquidStream.IsFlowing());
        UIManager.instance.EnablePullLeverTutorialImage(false);
        UIManager.instance.EnableHoldGlassTutorialImage(true);
        yield return new WaitUntil(() => drinkSystem.IsMoving());
        UIManager.instance.EnableHoldGlassTutorialImage(false);
        UIManager.instance.EnableMoveGlassTutorialImage(true);
        yield return new WaitUntil(() => drinkSystem.IsMovingForReal());
        UIManager.instance.EnableMoveGlassTutorialImage(false);
        UIManager.instance.EnableFillGlassTutorialImage(true);
        yield return new WaitUntil(() => (drinkSystem.GetBeerFill() < drinkSystem.GetMaxFill() && !liquidStream.IsFillingTheJug()) || drinkSystem.GetBeerFill() <= drinkSystem.GetMinFill());
        UIManager.instance.EnableFillGlassTutorialImage(false);
        UIManager.instance.EnableDrinkTutorialDirection(true);
        yield return new WaitUntil(() => drinkSystem.IsDrinking());
        UIManager.instance.EnableDrinkTutorialDirection(false);
        yield return new WaitUntil(() => drinkSystem.IsIdling() || drinkSystem.IsMoving());
        UIManager.instance.EnableDriveTutorial(true);
        yield return new WaitUntil(() => carController.GetLastMove() > 0);
        UIManager.instance.EnableDriveTutorial(false);
        yield return new WaitForSeconds(2);
        tutorial = false;
        StartCoroutine(UIManager.instance.FadeInDrinkNDrive());
    }
}
