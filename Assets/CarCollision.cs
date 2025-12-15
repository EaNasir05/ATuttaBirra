using UnityEngine;

public class PushOnCollision : MonoBehaviour
{
    [Header("Push Forces")]
    [Tooltip("Forza applicata all'oggetto")]
    public float objectPushForce = 10f;

    [Tooltip("Forza applicata al Player (solo asse X)")]
    public float playerPushForce = 5f;

    [Header("Push Speeds")]
    [Tooltip("Velocit� di spinta del Player (solo asse X)")]
    public float playerPushSpeed = 5f;

    [Tooltip("Velocit� di spinta dell'oggetto")]
    public float objectPushSpeed = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        Rigidbody objectRb = GetComponent<Rigidbody>();

        if (playerRb == null || objectRb == null)
            return;

        ContactPoint contact = collision.contacts[0];

       
        Vector3 pushDir = (collision.transform.position - contact.point).normalized;

        
        Vector3 playerPushDir = new Vector3(
            Mathf.Sign(pushDir.x), 
            0f,
            0f
        );

        
        playerRb.linearVelocity = playerPushDir * playerPushSpeed;

        
        objectRb.linearVelocity = -pushDir * objectPushSpeed;
    }
}