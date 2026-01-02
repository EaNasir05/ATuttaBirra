using UnityEngine;
using UnityEngine.InputSystem;

public class LeverInteraction_InputSystem : MonoBehaviour
{
    [Header("References")]
    public Transform playerRoot;
    public Transform leftHand;
    public Transform handTargetOnLever;
    public Transform leverPivot;
    public LiquidStreamToggle liquidStream;

    [Header("Player Movement")]
    public MonoBehaviour playerMovementScript;  

    [Header("Hand")]
    public float handMoveSpeed = 6f;
    public float handReturnSpeed = 6f;

    [Header("Lever")]
    public float maxAngle = -60f;
    public float leverSpeed = 90f;
    public float returnSpeed = 140f;
    public float angleTolerance = 1f;

    private float currentAngle = 0f;
    private bool isGrabbing = false;

    private Vector3 handStartLocalPos;
    private Quaternion handStartLocalRot;

    void Start()
    {
        handStartLocalPos = playerRoot.InverseTransformPoint(leftHand.position);
        handStartLocalRot = Quaternion.Inverse(playerRoot.rotation) * leftHand.rotation;
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return;

        bool grabbingNow =
            gamepad.leftShoulder.isPressed &&
            gamepad.leftTrigger.isPressed;

        
        if (grabbingNow != isGrabbing)
        {
            isGrabbing = grabbingNow;
            SetPlayerMovement(!isGrabbing);
        }

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

   

    void SetPlayerMovement(bool enabled)
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = enabled;
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
        Vector3 targetWorldPos = playerRoot.TransformPoint(handStartLocalPos);
        Quaternion targetWorldRot = playerRoot.rotation * handStartLocalRot;

        leftHand.position = Vector3.Lerp(
            leftHand.position,
            targetWorldPos,
            Time.deltaTime * handReturnSpeed
        );

        leftHand.rotation = Quaternion.Slerp(
            leftHand.rotation,
            targetWorldRot,
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
        leverPivot.localRotation =
            Quaternion.AngleAxis(currentAngle, Vector3.forward);
    }

    

    void UpdateFlow()
    {
        if (liquidStream == null)
            return;

        bool leverAtBottom =
            Mathf.Abs(currentAngle - maxAngle) <= angleTolerance;

        liquidStream.SetFlow(leverAtBottom);
    }
}

