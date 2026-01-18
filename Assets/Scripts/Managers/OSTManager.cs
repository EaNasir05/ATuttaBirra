using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class OSTManager : MonoBehaviour
{
    public static OSTManager instance;

    [SerializeField] private GameObject[] clips;
    [SerializeField] private float[] alcoolPowerStages;
    [SerializeField] private float ebrezzaMultiplier;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerSnapshot[] snapshots;
    [SerializeField] private DrinkSystem drinkSystem;
    private int currentLevel;
    private int activeMusicLayers;
    private float currentMuffling;

    private void Awake()
    {
        instance = this;
        currentLevel = 0;
        activeMusicLayers = 0;
        currentMuffling = 0;
        audioMixer.SetFloat("MusicVolume", 0);
    }

    void Update()
    {
        if (GameManager.instance.gameStarted)
        {
            UpdateActiveLayers();
            UpdateEbrezza();
        }
    }

    private void UpdateEbrezza()
    {
        float ebrezza = 22000 - (Mathf.Clamp(GameManager.instance.GetTotalBeerConsumed() - 1, 0, 10) * ebrezzaMultiplier);
        if (ebrezza != currentMuffling)
        {
            audioMixer.SetFloat("Ebrezza", ebrezza);
            currentMuffling = ebrezza;
        }
    }

    private void UpdateActiveLayers()
    {
        int layers = CalculateNewActiveLayers();
        if (layers > activeMusicLayers)
        {
            snapshots[layers].TransitionTo(0);
            activeMusicLayers = layers;
        }
        else if (layers < activeMusicLayers)
        {
            snapshots[layers].TransitionTo((activeMusicLayers - layers) * 1.5f);
            activeMusicLayers = layers;
        }
    }

    private int CalculateNewActiveLayers()
    {
        float power = GameManager.instance.GetAlcoolPower();
        if (power > alcoolPowerStages[4])
            return 4;
        if (power > alcoolPowerStages[3])
            return 3;
        if (power > alcoolPowerStages[2])
            return 2;
        if (power > alcoolPowerStages[1])
            return 1;
        return 0;
    }

    public void StartGame()
    {
        clips[0].SetActive(true);
    }

    public IEnumerator GameOver()
    {
        float elapsed = 0;
        yield return new WaitForSeconds(2);
        while (elapsed < 1)
        {
            elapsed += Time.deltaTime;
            audioMixer.SetFloat("MusicVolume", Mathf.Lerp(0, -80, elapsed / 1));
            yield return null;
        }
    }
}
