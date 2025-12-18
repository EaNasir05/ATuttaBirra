using UnityEngine;

public class BottleMovement : MonoBehaviour
{
    [Header("Movimento principale")]
    [Tooltip("Direzione di movimento (es: 1,0 = destra | -1,0 = sinistra)")]
    public Vector2 moveDirection = Vector2.right;

    [Tooltip("Velocità del movimento principale")]
    public float moveSpeed = 2f;
    public float accelerationMultiplier;

    [Header("Fluttuazione verticale")]
    public float floatAmplitude = 0.5f;  
    public float floatSpeed = 2f;        

    private Rigidbody rb;
    private float startY;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        startY = transform.position.y;
        moveDirection = moveDirection.normalized;  
    }

    void Update()
    {
        MoveLinear();
        FloatVertical();
        rb.linearVelocity = new Vector3(0, 0, GameManager.instance.GetAlcoolPower() * -accelerationMultiplier);
    }

    void MoveLinear()
    {
        
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void FloatVertical()
    {
        
        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }
}