using UnityEngine;

public class CarDestroyer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Car"))
            Destroy(collision.gameObject);
    }
}
