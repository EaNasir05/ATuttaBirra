using UnityEngine;

public class StripesSpawner : MonoBehaviour
{
    [SerializeField] private GameObject stripesPrefab;
    [SerializeField] private float stripesHeight;
    [SerializeField] private Transform firstStripesLinked;
    private Transform lastStripesSpawned;
    private Transform _t;

    private void Awake()
    {
        lastStripesSpawned = firstStripesLinked;
        _t = transform;
    }

    private void Update()
    {
        if (lastStripesSpawned.position.z < _t.position.z - 21f)
        {
            GameObject stripes = GameObject.Instantiate(stripesPrefab);
            lastStripesSpawned = stripes.transform;
            lastStripesSpawned.position = new Vector3(_t.position.x, stripesHeight, _t.position.z);
        }
    }
}
