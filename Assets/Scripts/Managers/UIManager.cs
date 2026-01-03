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
    [SerializeField] private int ebrezzaMultiplier;
    [SerializeField] private float maxEbrezzaBlend;
    [SerializeField] private FullScreenPassRendererFeature ebrezzaScreenRenderer;
    [SerializeField] private ParticleSystem speedEffect;
    [SerializeField] private float speedEffectMultiplier;
    [SerializeField] private CameraMovement cameraHandler;
    [SerializeField] private float maxCameraDistance = 1f;
    [SerializeField] private float cameraMovementMultiplier;
    private ParticleSystem.ShapeModule shape;

    private void Awake()
    {
        instance = this;
        ebrezzaScreenRenderer.passMaterial.SetFloat("_Blend", 0);
    }

    private void Update()
    {
        UpdateSpeedEffect();
    }

    private void UpdateSpeedEffect()
    {
        int increment = (int) ((GameManager.instance.GetAlcoolPower() - 1) * speedEffectMultiplier);
        float cameraDistance = Mathf.Clamp((GameManager.instance.GetAlcoolPower() - 1) * cameraMovementMultiplier, 0, maxCameraDistance);
        shape = speedEffect.shape;
        shape.radius = 25 - increment;
        cameraHandler.backwardDistance = cameraDistance;
    }

    public void UpdateEbrezza()
    {
        float blend = GameManager.instance.GetTotalBeerConsumed() / ebrezzaMultiplier;
        if (blend > maxEbrezzaBlend)
            blend = maxEbrezzaBlend;
        else if (blend < 0)
            blend = 0;
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
    }
}
