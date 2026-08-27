using UnityEngine;

public class ObstacleChecker : MonoBehaviour
{
    [SerializeField] private EntitiesSpawner spawner;

    private void OnTriggerExit(Collider collider)
    {
        if (collider.transform.CompareTag("Obstacle"))
        {
            spawner.ClearLane();
        }
    }
}
