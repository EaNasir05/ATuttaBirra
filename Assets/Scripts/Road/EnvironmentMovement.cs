using UnityEngine;

public class EnvironmentMovement : MonoBehaviour
{
    [SerializeField] private float accelerationMultiplier;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float speed = -accelerationMultiplier * GameManager.instance.GetAlcoolPower() < 0 ? -accelerationMultiplier * GameManager.instance.GetAlcoolPower() : 0;
        rb.linearVelocity = new Vector3(0, 0, speed);
    }
}
