using Unity.VisualScripting;
using UnityEngine;

public enum Direction { left, right };

public class CarMovement : MonoBehaviour
{
    public float currentSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float startingMovementDuration;
    [SerializeField] private float accelerationMultiplier;
    [SerializeField] private float skiddingMovementSpeed;
    [SerializeField] private float skiddingRotationSpeed;
    private Transform skiddingLeftTarget;
    private Transform skiddingRightTarget;
    private bool skidding;
    private Direction skiddingDirection;
    private Rigidbody rb;
    private BottleDetector bottleDetector;
    private CarDetector carDetector;

    public float GetBaseSpeed() => speed;
    public float GetAccelerationMultiplier() => accelerationMultiplier;

    private void Awake()
    {
        skiddingLeftTarget = GameObject.FindGameObjectWithTag("LeftSkiddingTarget").transform;
        skiddingRightTarget = GameObject.FindGameObjectWithTag("RightSkiddingTarget").transform;
        rb = GetComponent<Rigidbody>();
        bottleDetector = GetComponentInChildren<BottleDetector>();
        carDetector = GetComponentInChildren<CarDetector>();
    }

    private void Update()
    {
        if (!skidding)
        {
            float extraSpeed = GameManager.instance.GetAlcoolPower() * -accelerationMultiplier < 0 ? GameManager.instance.GetAlcoolPower() * -accelerationMultiplier : 0;
            float altSpeed = (-speed + extraSpeed) * (bottleDetector != null ? bottleDetector.speedMultiplier : 1);
            currentSpeed = carDetector != null && carDetector.speed > altSpeed ? carDetector.speed : altSpeed;
            if (!rb.isKinematic)
                rb.linearVelocity = new Vector3(0, 0, currentSpeed);
        }
        else
        {
            currentSpeed = 0;
            Transform skiddingTarget = skiddingDirection == Direction.left ? skiddingLeftTarget : skiddingRightTarget;
            float rotY = skiddingDirection == Direction.left ? -skiddingRotationSpeed : skiddingRotationSpeed;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + rotY * Time.deltaTime, transform.rotation.eulerAngles.z);
            transform.position = Vector3.MoveTowards(transform.position, skiddingTarget.position, skiddingMovementSpeed * Time.deltaTime);
        }
    }

    public void StartSkidding(Direction direction)
    {
        skiddingDirection = direction;
        skidding = true;
        rb.linearVelocity = Vector3.zero;
    }
}
