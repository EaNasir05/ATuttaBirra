using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Headhit")]

    [Tooltip("Quanto la camera avanza")]
    public float forwardDistance = 0.2f;

    [Tooltip("Gradi di rotazione in avanti")]
    public float rotationAngle = 25f;

    [Tooltip("Velocità movimento in avanti")]
    public float forwardSpeed = 2f;

    [Tooltip("Velocità ritorno alla posizione iniziale")]
    public float returnSpeed = 2f;

    [Tooltip("Offset del pivot verso il basso (valore negativo)")]
    public float pivotOffsetY = -0.3f;

    [Tooltip("Tempo di permanenza in posizione prima del ritorno")]
    public float holdTime = 0.5f;

    [Header("SCREENSHAKE")]
    [Tooltip("Durata screenshake")]
    public float shakeDuration = 0.15f;

    [Tooltip("Intensità screenshake")]
    public float shakeIntensity = 0.05f;

    [Header("Accelerazione")]

    [Tooltip("Distanza massima indietro")]
    public float backwardDistance = 0.1f;

    [Tooltip("Velocità movimento indietro")]
    public float backwardSpeed = 2f;

    [Tooltip("Velocità ritorno da movimento indietro")]
    public float backwardReturnSpeed = 4f;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    private bool isHeadHitPlaying = false;
    private float backwardCurrent = 0f;

    void Start()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }

    void Update()
    {
        HandleBackwardHold();
    }
    
    public IEnumerator HeadHitMovement()
    {
        isHeadHitPlaying = true;

        Vector3 worldPivot = transform.position + transform.up * pivotOffsetY;
        float t = 0f;

        
        while (t < 1f)
        {
            t += Time.deltaTime * forwardSpeed;
            float curve = Mathf.SmoothStep(0f, 1f, t);

            transform.localPosition =
                initialLocalPosition + transform.forward * forwardDistance * curve;

            transform.RotateAround(
                worldPivot,
                transform.right,
                rotationAngle * Time.deltaTime * forwardSpeed
            );

            yield return null;
        }

        
        if (holdTime > 0f)
        {
            if (shakeIntensity > 0f && shakeDuration > 0f)
                StartCoroutine(ScreenShake(shakeDuration, shakeIntensity));

            yield return new WaitForSeconds(holdTime);
        }

        t = 0f;

        
        while (t < 1f)
        {
            t += Time.deltaTime * returnSpeed;

            transform.localPosition =
                Vector3.Lerp(transform.localPosition, initialLocalPosition, t);

            transform.localRotation =
                Quaternion.Slerp(transform.localRotation, initialLocalRotation, t);

            yield return null;
        }

        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;

        isHeadHitPlaying = false;
    }

    
    void HandleBackwardHold()
    {
        if (isHeadHitPlaying)
            return;

        backwardCurrent = Mathf.MoveTowards(
            backwardCurrent,
            backwardDistance,
            Time.deltaTime * backwardSpeed
        );

        transform.localPosition =
            initialLocalPosition - transform.forward * backwardCurrent;
    }

    
    IEnumerator ScreenShake(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 offset = Random.insideUnitSphere * intensity;
            transform.localPosition += offset;

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}