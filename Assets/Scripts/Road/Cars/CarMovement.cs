using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float startingMovementDuration;
    [SerializeField] private float accelerationMultiplier;
    private Rigidbody rb;
    private float startingSpeed;
    private Vector3 startingSize;
    private float startingPosition;
    private Vector3 originalSize;
    private float originalPosition;
    private float elapsed;
    private float startingMovementRealDuration;

    private void Awake()
    {
        originalPosition = transform.position.y;
        originalSize = transform.localScale;
        startingPosition = transform.position.y / 10;
        transform.position = new Vector3(transform.position.x, startingPosition, transform.position.z);
        startingSize = new Vector3(originalSize.x / 10, originalSize.y / 10, originalSize.z / 10);
        transform.localScale = startingSize;
        startingSpeed = speed / 10;
        rb = GetComponent<Rigidbody>();
        elapsed = 0f;
        startingMovementRealDuration = startingMovementDuration / GameManager.instance.GetAlcoolPower();
    }

    private void Update()
    {
        if (elapsed < startingMovementRealDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / startingMovementDuration;
            float currentSpeed = Mathf.Lerp(startingSpeed, speed, t);
            Vector3 currentSize = Vector3.Lerp(startingSize, originalSize, t);
            float currentPosition = Mathf.Lerp(startingPosition, originalPosition, t);
            rb.linearVelocity = new Vector3(0, 0, currentSpeed * -1);
            transform.localScale = currentSize;
            transform.position = new Vector3(transform.position.x, currentPosition, transform.position.z);
        }
        else
        {
            float extraSpeed = (GameManager.instance.GetAlcoolPower() - 1) * accelerationMultiplier > 0 ? (GameManager.instance.GetAlcoolPower() - 1) * accelerationMultiplier : 0;
            rb.linearVelocity = new Vector3(0, 0, -speed + extraSpeed);
        }
    }
}
