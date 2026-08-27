using UnityEngine;

public class ObstaclesDestroyer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerExit(Collider collider)
    {
        if (collider.transform.CompareTag("Obstacle"))
        {
            Destroy(collider.gameObject);
        }
    }
}
