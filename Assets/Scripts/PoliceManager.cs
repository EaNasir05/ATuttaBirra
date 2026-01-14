using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PoliceChaseSystem : MonoBehaviour
{
    [SerializeField] private Image reflectionInTheMirror;
    [SerializeField] private Sprite[] policeReflections;
    [SerializeField] private Light[] lightsOnThePlayer;
    [SerializeField] private Light[] lightsOnThePolice;
    [SerializeField] private float lightDuration;
    [SerializeField] private float lightMinIntensity;
    [SerializeField] private float lightMaxIntensity;
    [SerializeField] private float reflectionMinSize;
    [SerializeField] private float reflectionMaxSize;
    [SerializeField] private float reflectionMaxPosY;
    [SerializeField] private float reflectionMinPosY;
    [SerializeField] private Transform policeCar;
    [SerializeField] private float policeMovementDuration;
    [SerializeField] private float reflectionTransitionDuration;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float maxAudioVolume;
    [SerializeField] private float minAudioVolume;
    private float timePassed = 0;
    private int currentColor = 0;
    private bool policeNear = false;
    private bool readyToUpdateReflection = false;
    private bool policeSpawned = false;
    private float policeAlcoolPower;
    private float minAlcoolPower;

    private void Awake()
    {
        reflectionInTheMirror.rectTransform.localScale = new Vector3(reflectionMinSize, reflectionMinSize, 1);
        reflectionInTheMirror.rectTransform.localPosition = new Vector3(0, reflectionMaxPosY, 0);
    }

    private void Start()
    {
        policeAlcoolPower = GameManager.instance.GetPoliceAlcoolPower();
        minAlcoolPower = GameManager.instance.GetMinAlcoolPower();
    }

    private void Update()
    {
        if (GameManager.instance.gameStarted)
            UpdateFeedbacks();
        if (policeNear)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= lightDuration)
            {
                UpdateLights();
                timePassed = 0;
            }
        }
    }

    private void UpdateFeedbacks()
    {
        if (!policeSpawned)
        {
            float alcoolPower = GameManager.instance.GetAlcoolPower();
            if (alcoolPower <= policeAlcoolPower && !policeNear)
            {
                policeNear = true;
                currentColor = 0;
                timePassed = lightDuration;
                lightsOnThePlayer[0].transform.parent.gameObject.SetActive(true);
                audioSource.Play();
                StartCoroutine(ApproachThePlayer());
            }
            else if (alcoolPower > policeAlcoolPower && policeNear)
            {
                policeNear = false;
                lightsOnThePlayer[0].transform.parent.gameObject.SetActive(false);
                audioSource.Stop();
                StartCoroutine(DepartFromThePlayer());
            }

            if (policeNear)
            {
                float t = Mathf.Clamp(alcoolPower - minAlcoolPower, 0, policeAlcoolPower - minAlcoolPower) * (1 / (policeAlcoolPower - minAlcoolPower));
                float lightIntensity = Mathf.Lerp(lightMaxIntensity, lightMinIntensity, t);
                audioSource.volume = Mathf.Lerp(maxAudioVolume, minAudioVolume, t);
                lightsOnThePlayer[0].intensity = lightIntensity;
                lightsOnThePlayer[1].intensity = lightIntensity;
                if (readyToUpdateReflection)
                {
                    float reflectionSize = Mathf.Lerp(reflectionMaxSize, reflectionMinSize, t);
                    float reflectionPosY = Mathf.Lerp(reflectionMinPosY, reflectionMaxPosY, t);
                    reflectionInTheMirror.rectTransform.localScale = new Vector3(reflectionSize, reflectionSize, 1);
                    reflectionInTheMirror.rectTransform.localPosition = new Vector3(0, reflectionPosY, 0);
                }
            }
        }
    }

    private IEnumerator ApproachThePlayer()
    {
        float elapsed = 0;
        float currentScale = reflectionInTheMirror.rectTransform.localScale.x;
        float targetScale = Mathf.Lerp(reflectionMaxSize, reflectionMinSize, Mathf.Clamp(GameManager.instance.GetAlcoolPower() - 0.5f, 0, 0.5f) * 2);
        float currentPosY = reflectionInTheMirror.rectTransform.localPosition.y;
        float targetPosY = Mathf.Lerp(reflectionMinPosY, reflectionMaxPosY, Mathf.Clamp(GameManager.instance.GetAlcoolPower() - 0.5f, 0, 0.5f) * 2);
        reflectionInTheMirror.gameObject.SetActive(true);
        while (elapsed < reflectionTransitionDuration && policeNear)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / reflectionTransitionDuration;
            float reflectionSize = Mathf.Lerp(currentScale, targetScale, t);
            float reflectionPosY = Mathf.Lerp(currentPosY, targetPosY, t);
            reflectionInTheMirror.rectTransform.localScale = new Vector3(reflectionSize, reflectionSize, 1);
            reflectionInTheMirror.rectTransform.localPosition = new Vector3(0, reflectionPosY, 0);
            yield return null;
        }
        readyToUpdateReflection = true;
    }

    private IEnumerator DepartFromThePlayer()
    {
        readyToUpdateReflection = false;
        float elapsed = 0;
        float currentScale = reflectionInTheMirror.rectTransform.localScale.x;
        float targetScale = reflectionMinSize;
        float currentPosY = reflectionInTheMirror.rectTransform.localPosition.y;
        float targetPosY = reflectionMaxPosY;
        while (elapsed < reflectionTransitionDuration && !policeNear)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / reflectionTransitionDuration;
            float reflectionSize = Mathf.Lerp(currentScale, targetScale, t);
            float reflectionPosY = Mathf.Lerp(currentPosY, targetPosY, t);
            reflectionInTheMirror.rectTransform.localScale = new Vector3(reflectionSize, reflectionSize, 1);
            reflectionInTheMirror.rectTransform.localPosition = new Vector3(0, reflectionPosY, 0);
            yield return null;
        }
        reflectionInTheMirror.gameObject.SetActive(false);
    }

    private void UpdateLights()
    {
        if (currentColor == 0)
        {
            if (policeSpawned)
            {
                lightsOnThePolice[0].enabled = false;
                lightsOnThePolice[1].enabled = true;
            }
            else
            {
                reflectionInTheMirror.sprite = policeReflections[1];
                lightsOnThePlayer[0].enabled = false;
                lightsOnThePlayer[1].enabled = true;
            }
            currentColor = 1;
        }
        else
        {
            if (policeSpawned)
            {
                lightsOnThePolice[1].enabled = false;
                lightsOnThePolice[0].enabled = true;
            }
            else
            {
                reflectionInTheMirror.sprite = policeReflections[0];
                lightsOnThePlayer[1].enabled = false;
                lightsOnThePlayer[0].enabled = true;
            }
            currentColor = 0;
        }
    }

    public IEnumerator SpawnPoliceCar()
    {
        policeSpawned = true;
        readyToUpdateReflection = false;
        float spawnX;
        float rotSign;
        if (transform.position.x >= 0)
        {
            spawnX = transform.position.x - 4;
            rotSign = 1;
        }
        else
        {
            spawnX = transform.position.x + 4;
            rotSign = -1;
        }
        float elapsed = 0;
        Vector3 currentScale = reflectionInTheMirror.rectTransform.localScale;
        Vector3 targetScale = new Vector3(reflectionMaxSize, reflectionMaxSize, 1);
        Vector3 currentPosition = reflectionInTheMirror.rectTransform.localPosition;
        Vector3 targetPosition = new Vector3(currentPosition.x + (260 * -rotSign), reflectionMinPosY, 0);
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            reflectionInTheMirror.rectTransform.localPosition = Vector3.Lerp(currentPosition, targetPosition, elapsed / 0.5f);
            reflectionInTheMirror.rectTransform.localScale = Vector3.Lerp(currentScale, targetScale, elapsed / 0.5f);
            yield return null;
        }
        lightsOnThePlayer[0].enabled = false;
        lightsOnThePlayer[1].enabled = false;
        policeCar.position = new Vector3(spawnX, 1.3f, -12);
        policeCar.gameObject.SetActive(true);
        float elapsedPosition = 0;
        float elapsedRotation = 0;
        float extraX = 0;
        while (elapsedPosition < policeMovementDuration)
        {
            elapsedPosition += Time.deltaTime;
            float tPos = elapsedPosition / policeMovementDuration;
            if (elapsedPosition >= policeMovementDuration / 3)
            {
                elapsedRotation += Time.deltaTime;
                float tRot = elapsedRotation / (policeMovementDuration * 0.66f);
                extraX = Mathf.Lerp(spawnX, transform.position.x, tRot);
                policeCar.rotation = Quaternion.Euler(0, 360 + (Mathf.Lerp(0, 90, tRot) * rotSign), 0);
            }
            extraX = extraX != 0 ? extraX : spawnX;
            policeCar.position = new Vector3(extraX, 1.3f, Mathf.Lerp(-12, -2, tPos));
            yield return null;
        }
        GameManager.instance.policeArrived = true;
    }
}
