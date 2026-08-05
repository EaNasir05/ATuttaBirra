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

    private Transform _t;
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    private bool isHeadHitPlaying = false;
    private float backwardCurrent = 0f;

    void Awake()
    {
        _t = transform;
    }

    void Start()
    {
        initialLocalPosition = _t.localPosition;
        initialLocalRotation = _t.localRotation;
    }

    void Update()
    {
        HandleBackwardHold();
    }
    
    public IEnumerator HeadHitMovement()
    {
        isHeadHitPlaying = true;

        Vector3 worldPivot = pivotOffsetY * _t.position + _t.up;
        float t = 0f;
        float deltaTime;
        
        while (t < 1f)
        {
            deltaTime = Time.deltaTime;
            t += deltaTime * forwardSpeed;
            float curve = Mathf.SmoothStep(0f, 1f, t);

            _t.localPosition =
                initialLocalPosition + forwardDistance * curve * _t.forward;

            _t.RotateAround(
                worldPivot,
                _t.right,
                rotationAngle * deltaTime * forwardSpeed
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
            deltaTime = Time.deltaTime;
            t += deltaTime * returnSpeed;

            _t.localPosition =
                Vector3.Lerp(_t.localPosition, initialLocalPosition, t);

            _t.localRotation =
                Quaternion.Slerp(_t.localRotation, initialLocalRotation, t);

            yield return null;
        }

        _t.localPosition = initialLocalPosition;
        _t.localRotation = initialLocalRotation;

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

        _t.localPosition =
            initialLocalPosition - _t.forward * backwardCurrent;
    }

    
    IEnumerator ScreenShake(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 offset = Random.insideUnitSphere * intensity;
            _t.localPosition += offset;

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}