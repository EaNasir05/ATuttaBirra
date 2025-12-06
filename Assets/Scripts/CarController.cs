

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

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        if (moveAction.action != null)
            moveAction.action.Enable();

        if (speedAction.action != null)
            speedAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction.action != null)
            moveAction.action.Disable();

        if (speedAction.action != null)
            speedAction.action.Disable();
    }

    void FixedUpdate()
    {
        Vector2 move = moveAction.action != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        Vector2 speed = speedAction.action != null ? speedAction.action.ReadValue<Vector2>() : Vector2.zero;

        
        float moveX = Mathf.Abs(move.x) > inputDeadzone ? move.x : 0f;

        
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
        float newVelX = Mathf.Lerp(currentVel.x, desiredVelX, 1f - Mathf.Exp(-accelSmoothing * Time.fixedDeltaTime));

        rb.linearVelocity = new Vector3(newVelX, currentVel.y, currentVel.z);
    }
}
