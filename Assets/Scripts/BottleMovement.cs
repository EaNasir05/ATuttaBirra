using UnityEngine;

public class BottleMovement : MonoBehaviour
{
    [Header("Movimento principale")]
    [Tooltip("Direzione di movimento (es: 1,0 = destra | -1,0 = sinistra)")]
    public Vector2 moveDirection = Vector2.right;

    [Tooltip("Velocità del movimento principale")]
    public float moveSpeed = 2f;

    [Header("Fluttuazione verticale")]
    public float floatAmplitude = 0.5f;  
    public float floatSpeed = 2f;        

    private float startY;

    void Start()
    {
        startY = transform.position.y;
        moveDirection = moveDirection.normalized;  
    }

    void Update()
    {
        MoveLinear();
        FloatVertical();
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