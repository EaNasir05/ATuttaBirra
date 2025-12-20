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
        float blend = (GameManager.instance.GetTotalBeerConsumed() - 1) / ebrezzaMultiplier;
        if (blend > maxEbrezzaBlend)
            blend = maxEbrezzaBlend;
        else if (blend < 0)
            blend = 0;
        Debug.Log("EBREZZA ATTUALE: " + blend);
        ebrezzaScreenRenderer.passMaterial.SetFloat("_Blend", blend);
    }

    public void UpdateBeerConsumed(float value)
    {
        beerConsumed.text = value + " L";
    }
}
