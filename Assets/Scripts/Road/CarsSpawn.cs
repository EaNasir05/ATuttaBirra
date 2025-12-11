using UnityEngine;

public class CarsSpawn : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    //private GameObject[] carPrefabs;
    [SerializeField] private float spawnTime;
    [SerializeField] private float spawnPositionZ;
    [SerializeField] private float spawnPositionY;
    [SerializeField] private float spawnMinPositionX;
    [SerializeField] private float spawnMaxPositionX;
    private float timePassed;

    void Awake()
    {
        timePassed = 0;
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= spawnTime)
        {
            float randomX = Random.Range(spawnMinPositionX, spawnMaxPositionX);
            //int i = Random.Range(0, carPrefabs.Count);
            //GameObject car = Instantiate(carPrefabs[i]);
            GameObject car = Instantiate(carPrefab);
            car.transform.position = new Vector3(randomX, spawnPositionY, spawnPositionZ);
            car.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            timePassed = 0;
        }
    }
}
