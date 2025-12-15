using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI elements")]
    [SerializeField] private TMP_Text beerConsumed;

    [Header("VFX")]
    [SerializeField] private int ebrezzaMultiplier;
    [SerializeField] private float maxEbrezzaBlend;
    [SerializeField] private FullScreenPassRendererFeature ebrezzaScreenRenderer;

    private void Awake()
    {
        instance = this;
        ebrezzaScreenRenderer.passMaterial.SetFloat("_Blend", 0);
    }

    public void UpdateEbrezza()
    {
        float blend = GameManager.instance.GetAlcoolPower() / ebrezzaMultiplier;
        if (blend > maxEbrezzaBlend)
            blend = maxEbrezzaBlend;
        Debug.Log("EBREZZA ATTUALE: " + blend);
        ebrezzaScreenRenderer.passMaterial.SetFloat("_Blend", blend);
    }

    public void UpdateBeerConsumed(float value)
    {
        beerConsumed.text = value + " L";
    }
}
