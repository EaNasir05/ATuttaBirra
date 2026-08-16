using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI elements")]
    [SerializeField] private TMP_Text beerConsumed;
    [SerializeField] private Image skyboxCover;
    [SerializeField] private Image blackScreen;
    [SerializeField] private GameObject holdGlassTutorialKeyboard;
    [SerializeField] private GameObject holdGlassTutorialDualShock;
    [SerializeField] private GameObject holdGlassTutorialXInput;
    [SerializeField] private GameObject moveGlassTutorialKeyboard;
    [SerializeField] private GameObject moveGlassTutorialDualShock;
    [SerializeField] private GameObject moveGlassTutorialXInput;
    [SerializeField] private GameObject fillGlassTutorial;
    [SerializeField] private GameObject holdLeverTutorialKeyboard;
    [SerializeField] private GameObject holdLeverTutorialDualShock;
    [SerializeField] private GameObject holdLeverTutorialXInput;
    [SerializeField] private GameObject pullLeverTutorialKeyboard;
    [SerializeField] private GameObject pullLeverTutorialDualShock;
    [SerializeField] private GameObject pullLeverTutorialXInput;
    [SerializeField] private GameObject drinkDirectionTutorial;
    [SerializeField] private GameObject driveTutorialGamepad;
    [SerializeField] private GameObject driveTutorialKeyboard;
    [SerializeField] private TMP_Text drinkAndDriveText;
    [SerializeField] private GameObject title;
    private GameObject currentActiveTutorial;

    [Header("VFX")]
    [SerializeField] private float blackScreenFadeDuration;
    public float ebbrezzaLevel = 0f;
    [SerializeField] private float maxEbbrezzaLevel = 0.5f;
    [SerializeField] private FullScreenPassRendererFeature ebrezzaScreenRenderer;
    [SerializeField] private ParticleSystem speedEffect;
    [SerializeField] private float speedEffectMultiplier;
    [SerializeField] private CameraMovement cameraHandler;
    [SerializeField] private float cameraDistanceWhileAccelerating = 0.2f;
    [SerializeField] private float cameraFirstMovementDuration = 0.2f;
    [SerializeField] private float cameraSecondMovementDuration = 0.5f;
    [SerializeField] private ParticleSystem fireEffect;
    //[SerializeField] private float fireEffectMultiplier;
    //[SerializeField] private float maxFireEffectIntensity;

    [Header("Beer popups")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private GameObject beerImagePrefab;
    [SerializeField] private Vector2 minMaxSize = new Vector2(80, 160);

    private int lastSpawnedLiter = 0;
    private ParticleSystem.ShapeModule speedShape;
    private Color fogColor;
    private float fogDensity;
    private Coroutine moveCameraRoutine;

    private void Awake()
    {
        instance = this;
        ebrezzaScreenRenderer.passMaterial.SetFloat("_Blend", 0);
        fogColor = RenderSettings.fogColor;
        fogDensity = RenderSettings.fogDensity;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.2f;
        speedShape = speedEffect.shape;
        speedShape.radius = 30;
        //fireEffect.gameObject.SetActive(false);
        cameraHandler.backwardDistance = 0;
    }

    private void Start()
    {
        StartCoroutine(FadeInBlackScreen());
    }

    private void Update()
    {
        if (GameManager.instance.gameStarted)
        {
            UpdateSpeedEffect();
        }
    }

    private IEnumerator FadeInBlackScreen()
    {
        float elapsed = 0;
        Color targetColor = new Color(Color.black.r, Color.black.g, Color.black.b, 0);
        while (elapsed < blackScreenFadeDuration)
        {
            elapsed += Time.deltaTime;
            blackScreen.color = Color.Lerp(Color.black, targetColor, elapsed / blackScreenFadeDuration);
            yield return null;
        }
        blackScreen.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutBlackScreen()
    {
        float elapsed = 0;
        yield return new WaitForSeconds(2);
        Color currentColor = new Color(Color.black.r, Color.black.g, Color.black.b, 0);
        blackScreen.color = currentColor;
        blackScreen.gameObject.SetActive(true);
        while (elapsed < blackScreenFadeDuration)
        {
            elapsed += Time.deltaTime;
            blackScreen.color = Color.Lerp(currentColor, Color.black, elapsed / blackScreenFadeDuration);
            yield return null;
        }
        SceneManager.LoadScene("GameOver");
    }

    public void StartGame()
    {
        StartCoroutine(ChangeFog());
        StartCoroutine(FadeOutTitle());
        //fireEffect.gameObject.SetActive(true);
    }

    public IEnumerator FadeOutTitle()
    {
        float elapsed = 0f;
        RectTransform rectTransform = title.GetComponent<RectTransform>();
        float size = rectTransform.localScale.x;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float currentSize = Mathf.Lerp(size, 0.01f, elapsed / 0.5f);
            rectTransform.localScale = new Vector2(currentSize, currentSize);
            yield return null;
        }
        title.SetActive(false);
    }

    private IEnumerator ChangeFog()
    {
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1f;
            RenderSettings.fogColor = Color.Lerp(Color.black, fogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(0.2f, fogDensity, t);
            //skyboxCover.color = new Color(skyboxCover.color.r, skyboxCover.color.g, skyboxCover.color.b, Mathf.Lerp(1, 0, t));
            yield return null;
        }
    }

    private void UpdateSpeedEffect()
    {
        float increment = GameManager.instance.IsImmuneToDeceleration() ? ((GameManager.instance.GetAlcoolPower() - 1) * speedEffectMultiplier) + 1 : ((GameManager.instance.GetAlcoolPower() - 1) * speedEffectMultiplier);
        speedShape.radius = 24 - increment;
    }

    public void UpdateEbbrezza(float increment)
    {
        ebbrezzaLevel += increment; //(GameManager.instance.GetTotalBeerConsumed() - 1) / ebrezzaDivider;
        ebbrezzaLevel = Mathf.Clamp(ebbrezzaLevel, 0, maxEbbrezzaLevel);
        ebrezzaScreenRenderer.passMaterial.SetFloat("_Blend", ebbrezzaLevel);
    }

    public void StartCameraMovement(float duration)
    {
        if (moveCameraRoutine != null)
            StopCoroutine(moveCameraRoutine);
        moveCameraRoutine = StartCoroutine(MoveHead(duration));
    }

    private IEnumerator MoveHead(float duration)
    {
        float elapsed = 0f;
        float currentDistance = cameraHandler.backwardDistance;
        while (elapsed < cameraFirstMovementDuration)
        {
            elapsed += Time.deltaTime;
            cameraHandler.backwardDistance = Mathf.Lerp(currentDistance, cameraDistanceWhileAccelerating, elapsed / cameraFirstMovementDuration);
            yield return null;
        }
        elapsed = 0f;
        yield return new WaitForSeconds(duration);
        while (elapsed < cameraFirstMovementDuration)
        {
            elapsed += Time.deltaTime;
            cameraHandler.backwardDistance = Mathf.Lerp(cameraDistanceWhileAccelerating, 0, elapsed / cameraFirstMovementDuration);
            yield return null;
        }
        moveCameraRoutine = null;
    }

    public void UpdateBeerConsumed(float value)
    {
        beerConsumed.text = value + "\npints";
    }

    public void GameOver()
    {
        speedShape.radius = 30;
        StartCoroutine(FadeOutBlackScreen());
    }
    public void CheckBeerPopups(float totalBeer)
    {
       
        if (beerImagePrefab == null || canvasRect == null)
            return;

        int currentLiters = Mathf.FloorToInt(totalBeer);

        if (currentLiters > lastSpawnedLiter)
        {
            int toSpawn = currentLiters - lastSpawnedLiter;

            for (int i = 0; i < toSpawn; i++)
            {
                SpawnBeerImage();
            }

            lastSpawnedLiter = currentLiters;
        }
    }

    private void SpawnBeerImage()
    {
        if (beerImagePrefab == null || canvasRect == null)
            return;

        GameObject img = Instantiate(beerImagePrefab, canvasRect);

        RectTransform rt = img.GetComponent<RectTransform>();

        float x = Random.Range(0, canvasRect.rect.width);
        float y = Random.Range(0, canvasRect.rect.height);

        rt.anchoredPosition = new Vector2(x, y);

        float size = Random.Range(minMaxSize.x, minMaxSize.y);
        rt.sizeDelta = new Vector2(size, size);
    }

    public IEnumerator FadeInDrinkNDrive()
    {
        float elapsed = 0f;
        float size = drinkAndDriveText.fontSize;
        drinkAndDriveText.fontSize = 5;
        drinkAndDriveText.gameObject.SetActive(true);
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float currentSize = Mathf.Lerp(5, size, elapsed / 0.5f);
            drinkAndDriveText.fontSize = currentSize;
            yield return null;
        }
        yield return new WaitForSeconds(2);
        drinkAndDriveText.gameObject.SetActive(false);
        GameManager.instance.gameStarted = true;
        StaticGameVariables.instance.firstTimePlaying = false;
    }

    public void EnableHoldGlassTutorialImage(bool value, DeviceType inputSystem)
    {
        switch (inputSystem)
        {
            case DeviceType.KeyboardMouse:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = holdGlassTutorialKeyboard;
                }
                holdGlassTutorialKeyboard.SetActive(value);
                break;
            case DeviceType.PSController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = holdGlassTutorialDualShock;
                }
                holdGlassTutorialDualShock.SetActive(value);
                break;
            case DeviceType.XboxController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = holdGlassTutorialXInput;
                }
                holdGlassTutorialXInput.SetActive(value);
                break;
            default:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = holdGlassTutorialXInput;
                }
                holdGlassTutorialXInput.SetActive(value);
                break;
        }
    }

    public void EnableMoveGlassTutorialImage(bool value, DeviceType inputSystem)
    {
        switch (inputSystem)
        {
            case DeviceType.KeyboardMouse:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = moveGlassTutorialKeyboard;
                }
                moveGlassTutorialKeyboard.SetActive(value);
                break;
            case DeviceType.PSController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = moveGlassTutorialDualShock;
                }
                moveGlassTutorialDualShock.SetActive(value);
                break;
            case DeviceType.XboxController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = moveGlassTutorialXInput;
                }
                moveGlassTutorialXInput.SetActive(value);
                break;
            default:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = moveGlassTutorialXInput;
                }
                moveGlassTutorialXInput.SetActive(value);
                break;
        }
    }

    public void EnableHoldLeverTutorialImage(bool value, DeviceType inputSystem)
    {
        switch (inputSystem)
        {
            case DeviceType.KeyboardMouse:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = holdLeverTutorialKeyboard;
                }
                holdLeverTutorialKeyboard.SetActive(value);
                break;
            case DeviceType.PSController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = holdLeverTutorialDualShock;
                }
                holdLeverTutorialDualShock.SetActive(value);
                break;
            case DeviceType.XboxController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = holdLeverTutorialXInput;
                }
                holdLeverTutorialXInput.SetActive(value);
                break;
            default:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = holdLeverTutorialXInput;
                }
                holdLeverTutorialXInput.SetActive(value);
                break;
        }
    }

    public void EnablePullLeverTutorialImage(bool value, DeviceType inputSystem)
    {
        switch (inputSystem)
        {
            case DeviceType.KeyboardMouse:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = pullLeverTutorialKeyboard;
                }
                pullLeverTutorialKeyboard.SetActive(value);
                break;
            case DeviceType.PSController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = pullLeverTutorialDualShock;
                }
                pullLeverTutorialDualShock.SetActive(value);
                break;
            case DeviceType.XboxController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = pullLeverTutorialXInput;
                }
                pullLeverTutorialXInput.SetActive(value);
                break;
            default:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = pullLeverTutorialXInput;
                }
                pullLeverTutorialXInput.SetActive(value);
                break;
        }
    }

    public void EnableDriveTutorial(bool value, DeviceType inputSystem)
    {
        switch (inputSystem)
        {
            case DeviceType.KeyboardMouse:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = driveTutorialKeyboard;
                }
                driveTutorialKeyboard.SetActive(value);
                break;
            case DeviceType.PSController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = driveTutorialGamepad;
                }
                driveTutorialGamepad.SetActive(value);
                break;
            case DeviceType.XboxController:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = driveTutorialGamepad;
                }
                driveTutorialGamepad.SetActive(value);
                break;
            default:
                if (value)
                {
                    currentActiveTutorial?.SetActive(false);
                    currentActiveTutorial = driveTutorialGamepad;
                }
                driveTutorialGamepad.SetActive(value);
                break;
        }
    }

    public void EnableFillGlassTutorialImage(bool value) => fillGlassTutorial.SetActive(value);
    public void EnableDrinkTutorialDirection(bool value) => drinkDirectionTutorial.SetActive(value);
    public void EnableTitle(bool value) => title.SetActive(value);

    public float GetMaxEbbrezzaLevel() => maxEbbrezzaLevel;
}
