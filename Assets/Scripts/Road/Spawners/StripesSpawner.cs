using UnityEngine;

public class StripesSpawner : MonoBehaviour
{
    [SerializeField] private GameObject stripesPrefab;
    [SerializeField] private float stripesHeight;
    [SerializeField] private Transform firstStripesLinked;
    private Transform lastStripesSpawned;

    private void Awake()
    {
        lastStripesSpawned = firstStripesLinked;
    }

    private void Update()
    {
        if (lastStripesSpawned.position.z < transform.position.z - 21f)
        {
            GameObject stripes = GameObject.Instantiate(stripesPrefab);
            lastStripesSpawned = stripes.transform;
            lastStripesSpawned.position = new Vector3(transform.position.x, stripesHeight, transform.position.z);
        }
    }
}
