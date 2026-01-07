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
    private float timePassed = 0;
    private int currentColor = 0;
    private bool policeNear = false;
    private bool policeSpawned = false;

    private void Awake()
    {

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
            if (alcoolPower <= 1 && !policeNear)
            {
                policeNear = true;
                currentColor = 0;
                timePassed = lightDuration;
                lightsOnThePlayer[0].transform.parent.gameObject.SetActive(true);
                reflectionInTheMirror.gameObject.SetActive(true);
            }
            else if (alcoolPower > 1 && policeNear)
            {
                policeNear = false;
                lightsOnThePlayer[0].transform.parent.gameObject.SetActive(false);
                reflectionInTheMirror.gameObject.SetActive(false);
            }

            if (policeNear)
            {
                float t = Mathf.Clamp(alcoolPower - 0.5f, 0, 0.5f) * 2;
                float lightIntensity = Mathf.Lerp(lightMaxIntensity, lightMinIntensity, t);
                float reflectionSize = Mathf.Lerp(reflectionMaxSize, reflectionMinSize, t);
                float reflectionPosY = Mathf.Lerp(reflectionMinPosY, reflectionMaxPosY, t);
                lightsOnThePlayer[0].intensity = lightIntensity;
                lightsOnThePlayer[1].intensity = lightIntensity;
                reflectionInTheMirror.rectTransform.localScale = new Vector3(reflectionSize, reflectionSize, 1);
                reflectionInTheMirror.rectTransform.localPosition = new Vector3(0, reflectionPosY, 0);
            }
        }
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
        reflectionInTheMirror.gameObject.SetActive(false);
        lightsOnThePlayer[0].enabled = false;
        lightsOnThePlayer[1].enabled = false;
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
