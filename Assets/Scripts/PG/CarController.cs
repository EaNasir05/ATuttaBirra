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
    public float rotationAngle = 10f;
    public float rotationSpeed = 5f;
    private Quaternion startingLocalRot;
    private float currentRotation;
    private Vector3 localUpAxis;

    [Header("Gamepad Vibration")]
    public float vibrationLow = 0.2f;
    public float vibrationHigh = 0.5f;
    public float vibrationDuration = 0.2f;

    private Rigidbody rb;
    private DrinkSystem drinkSystem;

    private float vibrationTimer = 0f;
    private bool vibrating = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        drinkSystem = GetComponentInChildren<DrinkSystem>();
        startingLocalRot = carTransform.localRotation;
        localUpAxis = startingLocalRot * Vector3.up;
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
        float movement = Move();
        Rotate(movement);
        UpdateVibration();
    }

    private float Move()
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

        return moveX;
    }

    private void Rotate(float movement)
    {
        float targetOffset = movement * rotationAngle;
        float speed = movement != 0 ? rotationSpeed : rotationSpeed / 2;

        currentRotation = Mathf.MoveTowards(
            currentRotation,
            targetOffset,
            speed * Time.fixedDeltaTime
        );

        Quaternion yawOffset = Quaternion.AngleAxis(currentRotation, localUpAxis);

        carTransform.localRotation = startingLocalRot * yawOffset;
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