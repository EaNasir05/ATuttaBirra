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
    [SerializeField] private AudioClip skiddingAudioClip;
    [SerializeField] private float skiddingAudioVolume;
    private Transform skiddingLeftTarget;
    private Transform skiddingRightTarget;
    private bool skidding;
    private Direction skiddingDirection;
    private Transform _t;
    private Rigidbody rb;
    private BottleDetector bottleDetector;
    private CarDetector carDetector;
    private AudioSource audioSource;

    public float GetBaseSpeed() => speed;
    public float GetAccelerationMultiplier() => accelerationMultiplier;

    private void Awake()
    {
        _t = transform;
        rb = GetComponent<Rigidbody>();
        bottleDetector = GetComponentInChildren<BottleDetector>();
        carDetector = GetComponentInChildren<CarDetector>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        skiddingLeftTarget = GameObject.FindGameObjectWithTag("LeftSkiddingTarget").transform;
        skiddingRightTarget = GameObject.FindGameObjectWithTag("RightSkiddingTarget").transform;
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
            _t.rotation = Quaternion.Euler(_t.rotation.eulerAngles.x, _t.rotation.eulerAngles.y + rotY * Time.deltaTime, _t.rotation.eulerAngles.z);
            _t.position = Vector3.MoveTowards(_t.position, skiddingTarget.position, skiddingMovementSpeed * Time.deltaTime);
        }
    }

    public void StartSkidding(Direction direction)
    {
        skiddingDirection = direction;
        SFXManager.instance.PlayClipWithRandomPitch(skiddingAudioClip, skiddingAudioVolume);
        skidding = true;
        rb.linearVelocity = Vector3.zero;
    }
}
