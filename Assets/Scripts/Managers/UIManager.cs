using System.Collections;
using TMPro;
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

    [Header("VFX")]
    [SerializeField] private float blackScreenFadeDuration;
    [SerializeField] private int ebrezzaDivider;
    [SerializeField] private float maxEbrezzaBlend;
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
        fireEffect.gameObject.SetActive(false);
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
        fireEffect.gameObject.SetActive(true);
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
        speedShape.radius = 25 - increment;
    }

    public void UpdateEbrezza()
    {
        float blend = (GameManager.instance.GetTotalBeerConsumed() - 1) / ebrezzaDivider;
        blend = Mathf.Clamp(blend, 0, maxEbrezzaBlend);
        ebrezzaScreenRenderer.passMaterial.SetFloat("_Blend", blend);
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
}
