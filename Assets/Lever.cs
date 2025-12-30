using UnityEngine;
using UnityEngine.InputSystem;

public class LeverInteraction_InputSystem : MonoBehaviour
{
    [Header("References")]
    public Transform leftHand;
    public Transform handTargetOnLever;
    public Transform leverPivot;
    public LiquidStreamToggle liquidStream;

    [Header("Hand")]
    public float handMoveSpeed = 6f;
    public float handReturnSpeed = 6f;

    [Header("Lever")]
    public float maxAngle = -60f;
    public float leverSpeed = 90f;
    public float returnSpeed = 140f;
    public float flowStartAngle = -10f;

    private float currentAngle = 0f;
    private bool isGrabbing = false;

    private Vector3 handStartPosition;
    private Quaternion handStartRotation;

    void Start()
    {
       
        handStartPosition = leftHand.position;
        handStartRotation = leftHand.rotation;
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return;

        isGrabbing = gamepad.leftShoulder.isPressed && gamepad.leftTrigger.isPressed;

        if (isGrabbing)
        {
            MoveHandToLever();
            HandleLeverInput(gamepad);
        }
        else
        {
            ReturnHand();
            ReturnLever();
        }

        UpdateLeverRotation();
        UpdateFlow();
    }

    

    void MoveHandToLever()
    {
        leftHand.position = Vector3.Lerp(
            leftHand.position,
            handTargetOnLever.position,
            Time.deltaTime * handMoveSpeed
        );

        leftHand.rotation = Quaternion.Slerp(
            leftHand.rotation,
            handTargetOnLever.rotation,
            Time.deltaTime * handMoveSpeed
        );
    }

    void ReturnHand()
    {
        leftHand.position = Vector3.Lerp(
            leftHand.position,
            handStartPosition,
            Time.deltaTime * handReturnSpeed
        );

        leftHand.rotation = Quaternion.Slerp(
            leftHand.rotation,
            handStartRotation,
            Time.deltaTime * handReturnSpeed
        );
    }

    

    void HandleLeverInput(Gamepad gamepad)
    {
        float stickY = gamepad.leftStick.y.ReadValue();

        if (stickY < -0.2f)
        {
            currentAngle += stickY * leverSpeed * Time.deltaTime;
            currentAngle = Mathf.Clamp(currentAngle, maxAngle, 0f);
        }
    }

    void ReturnLever()
    {
        currentAngle = Mathf.MoveTowards(
            currentAngle,
            0f,
            returnSpeed * Time.deltaTime
        );
    }

    void UpdateLeverRotation()
    {
        
        leverPivot.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);
    }

   

    void UpdateFlow()
    {
        if (liquidStream == null)
            return;

        bool shouldFlow = currentAngle <= flowStartAngle;
        liquidStream.SetFlow(shouldFlow);
    }
}
