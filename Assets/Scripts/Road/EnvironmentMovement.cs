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
        rb.linearVelocity = new Vector3(0, 0, -accelerationMultiplier * GameManager.instance.GetAlcoolPower());
    }
}
