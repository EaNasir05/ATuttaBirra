using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI elements")]
    [SerializeField] private TMP_Text beerConsumed;
    [SerializeField] private GameObject gameOverTab;

    [Header("VFX")]
    [SerializeField] private int ebrezzaDivider;
    [SerializeField] private float maxEbrezzaBlend;
    [SerializeField] private FullScreenPassRendererFeature ebrezzaScreenRenderer;
    [SerializeField] private ParticleSystem speedEffect;
    [SerializeField] private float speedEffectMultiplier;
    [SerializeField] private CameraMovement cameraHandler;
    [SerializeField] private float maxCameraDistance = 1f;
    [SerializeField] private float cameraMovementMultiplier;
    [SerializeField] private ParticleSystem fireEffect;
    //[SerializeField] private float fireEffectMultiplier;
    //[SerializeField] private float maxFireEffectIntensity;
    private ParticleSystem.ShapeModule speedShape;
    private Color fogColor;
    private float fogDensity;

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
    }

    private void Update()
    {
        if (GameManager.instance.gameStarted)
        {
            UpdateSpeedEffect();
        }
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
            yield return null;
        }
    }

    private void UpdateSpeedEffect()
    {
        float increment = GameManager.instance.IsImmuneToDeceleration() ? ((GameManager.instance.GetAlcoolPower() - 1) * speedEffectMultiplier) + 1 : ((GameManager.instance.GetAlcoolPower() - 1) * speedEffectMultiplier);
        float cameraDistance = GameManager.instance.IsImmuneToDeceleration() ? Mathf.Clamp((GameManager.instance.GetAlcoolPower() - 1) * cameraMovementMultiplier, 0, maxCameraDistance) + 0.02f : Mathf.Clamp((GameManager.instance.GetAlcoolPower() - 1) * cameraMovementMultiplier, 0, maxCameraDistance);
        speedShape.radius = 26 - increment;
        cameraHandler.backwardDistance = cameraDistance;
    }

    public void UpdateEbrezza()
    {
        float blend = (GameManager.instance.GetTotalBeerConsumed() - 1) / ebrezzaDivider;
        blend = Mathf.Clamp(blend, 0, maxEbrezzaBlend);
        ebrezzaScreenRenderer.passMaterial.SetFloat("_Blend", blend);
    }

    public void UpdateBeerConsumed(float value)
    {
        beerConsumed.text = value + " L";
    }

    public void GameOver()
    {
        if (gameOverTab != null)
            gameOverTab.SetActive(true);
        speedShape.radius = 30;
    }
}
