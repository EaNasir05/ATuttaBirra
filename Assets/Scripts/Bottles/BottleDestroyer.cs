using UnityEngine;

public class BottleDestroyer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Bottle"))
        {
            Destroy(collision.gameObject);
        }
    }
}
