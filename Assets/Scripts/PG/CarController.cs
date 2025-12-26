using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionProperty moveAction;
    public InputActionProperty speedAction;

    [Header("Movement")]
    public float baseSpeed = 5f;
    public float accelMultiplier = 1.5f;
    public float accelSmoothing = 10f;
    public float inputDeadzone = 0.15f;
    public float sameDirectionDotThreshold = 0.6f;

    [Header("Car Rotation")]
    [SerializeField] Transform carTransform;
    public float startingRotX = -90f;
    public float startingRotZ = -180f;
    public float rotationAngle = 10f;
    public float rotationSpeed = 5f;

    [Header("Auto Flip Rotation")]
    public float delayBeforeFlip = 1f;   
    public float flipAngle = 20f;        
    public float flipRotationSpeed = 8f; 

    [Header("Gamepad Vibration")]
    public float vibrationLow = 0.2f;
    public float vibrationHigh = 0.5f;
    public float vibrationDuration = 0.2f;

    private Rigidbody rb;
    private DrinkSystem drinkSystem;

    
    private float moveTimer = 0f;
    private bool hasFlipped = false;

    private float vibrationTimer = 0f;
    private bool vibrating = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        drinkSystem = GetComponentInChildren<DrinkSystem>();
    }

    void OnEnable()
    {
        moveAction.action?.Enable();
        speedAction.action?.Enable();
    }

    void OnDisable()
    {
        moveAction.action?.Disable();
        speedAction.action?.Disable();
        StopVibration();
    }


    void FixedUpdate()
    {
        Vector2 move = moveAction.action != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        Vector2 speed = speedAction.action != null && drinkSystem.IsIdling() ? speedAction.action.ReadValue<Vector2>() : Vector2.zero;

        float moveX = Mathf.Abs(move.x) > inputDeadzone ? move.x : 0f;

        if (moveX == 0f && speed != Vector2.zero)
        {
            moveX = Mathf.Abs(speed.x) > inputDeadzone ? speed.x : 0f; ;
            speed = move;
        }

        bool rightActive = speed.magnitude > inputDeadzone;
        bool sameDirection = false;
        if (rightActive && moveX != 0f)
        {
            Vector2 moveDir = new Vector2(Mathf.Sign(moveX), 0f);
            Vector2 speedDir = speed.normalized;
            float dot = Vector2.Dot(moveDir, speedDir);
            sameDirection = dot >= sameDirectionDotThreshold;
        }

        float targetSpeed = baseSpeed;
        if (sameDirection)
        {
            float rightMag = Mathf.Clamp01(speed.magnitude);
            targetSpeed = baseSpeed * (1f + accelMultiplier * rightMag);
        }

        float desiredVelX = moveX * targetSpeed;
        Vector3 currentVel = rb.linearVelocity;
        float newVelX = Mathf.Lerp(
            currentVel.x,
            desiredVelX,
            1f - Mathf.Exp(-accelSmoothing * Time.fixedDeltaTime)
        );

        rb.linearVelocity = new Vector3(newVelX, currentVel.y, currentVel.z);


        
        if (moveX != 0f)
        {
            moveTimer += Time.fixedDeltaTime;

            
            if (!hasFlipped && moveTimer >= delayBeforeFlip)
            {
                hasFlipped = true;
                StartVibration();
            }

            float targetRotation = -moveX * rotationAngle;

            
            float appliedRotationSpeed = hasFlipped ? flipRotationSpeed : rotationSpeed;

            if (hasFlipped)
            {
                targetRotation += moveX * flipAngle;
            }

            float currentRotation = carTransform.eulerAngles.y;

            float smoothRotation = Mathf.LerpAngle(
                currentRotation,
                targetRotation,
                appliedRotationSpeed * Time.fixedDeltaTime
            );

            carTransform.eulerAngles = new Vector3(startingRotX, smoothRotation, startingRotZ);
        }
        else
        {
            
            moveTimer = 0f;
            hasFlipped = false;

            float currentRotation = carTransform.eulerAngles.y;
            float smoothRotation = Mathf.LerpAngle(
                currentRotation,
                0f,
                rotationSpeed * Time.fixedDeltaTime
            );

            carTransform.eulerAngles = new Vector3(startingRotX, smoothRotation, startingRotZ);
        }

        UpdateVibration();
    }

   

    void StartVibration()
    {
        if (Gamepad.current == null) return;

        Gamepad.current.SetMotorSpeeds(vibrationLow, vibrationHigh);
        vibrating = true;
        vibrationTimer = 0f;
    }

    void UpdateVibration()
    {
        if (!vibrating) return;

        vibrationTimer += Time.deltaTime;

        if (vibrationTimer >= vibrationDuration)
        {
            StopVibration();
        }
    }

    void StopVibration()
    {
        if (Gamepad.current == null) return;

        Gamepad.current.SetMotorSpeeds(0f, 0f);
        vibrating = false;
    }
}