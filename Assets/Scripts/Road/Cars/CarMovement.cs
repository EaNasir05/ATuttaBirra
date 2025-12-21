using UnityEngine;

public enum Direction { left, right };

public class CarMovement : MonoBehaviour
{
    public float currentSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float startingMovementDuration;
    [SerializeField] private float accelerationMultiplier;
    private Transform skiddingLeftTarget;
    private Transform skiddingRightTarget;
    private bool skidding;
    private Direction skiddingDirection;
    private Rigidbody rb;
    private BottleDetector bottleDetector;
    private float startingSpeed;
    private Vector3 startingSize;
    private float startingPosition;
    private Vector3 originalSize;
    private float originalPosition;
    private float elapsed;
    private float startingMovementRealDuration;

    private void Awake()
    {
        /*
        originalPosition = transform.position.y;
        originalSize = transform.localScale;
        startingPosition = transform.position.y / 10;
        transform.position = new Vector3(transform.position.x, startingPosition, transform.position.z);
        startingSize = new Vector3(originalSize.x / 10, originalSize.y / 10, originalSize.z / 10);
        transform.localScale = startingSize;
        startingSpeed = speed / 10;
        elapsed = 0f;
        startingMovementRealDuration = startingMovementDuration / GameManager.instance.GetAlcoolPower();
        */
        rb = GetComponent<Rigidbody>();
        bottleDetector = GetComponentInChildren<BottleDetector>();
    }

    private void Update()
    {
        /*
        if (elapsed < startingMovementRealDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / startingMovementDuration;
            float currentStandardSpeed = Mathf.Lerp(startingSpeed, speed, t);
            Vector3 currentSize = Vector3.Lerp(startingSize, originalSize, t);
            float currentPosition = Mathf.Lerp(startingPosition, originalPosition, t);
            rb.linearVelocity = new Vector3(0, 0, currentStandardSpeed * -1);
            transform.localScale = currentSize;
            transform.position = new Vector3(transform.position.x, currentPosition, transform.position.z);
        }
        else
        {
            float extraSpeed = (GameManager.instance.GetAlcoolPower() - 1) * accelerationMultiplier > 0 ? (GameManager.instance.GetAlcoolPower() - 1) * accelerationMultiplier : 0;
            rb.linearVelocity = new Vector3(0, 0, -speed + extraSpeed);
        }
        */
        if (!skidding)
        {
            float extraSpeed = GameManager.instance.GetAlcoolPower() * -accelerationMultiplier < 0 ? GameManager.instance.GetAlcoolPower() * -accelerationMultiplier : 0;
            currentSpeed = (-speed + extraSpeed) * bottleDetector.speedMultiplier;
            rb.linearVelocity = new Vector3(0, 0, (-speed + extraSpeed) * bottleDetector.speedMultiplier);
        }
        else
        {
            Transform skiddingTarget = skiddingDirection == Direction.left ? skiddingLeftTarget : skiddingRightTarget;
        }
    }

    public void StartSkidding(Direction direction)
    {
        skiddingDirection = direction;
        skidding = true;
        rb.linearVelocity = Vector3.zero;
    }

    public void SetSkiddingTargets(Transform leftTarget, Transform rightTarget)
    {
        skiddingLeftTarget = leftTarget;
        skiddingRightTarget = rightTarget;
    }
}
