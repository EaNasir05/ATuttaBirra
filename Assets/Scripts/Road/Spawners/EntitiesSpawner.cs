using UnityEngine;

public class EntitiesSpawner : MonoBehaviour
{
    [SerializeField] private CarsList carsList;
    [SerializeField] private float spawnTime;
    [SerializeField] private float spawnPositionZ;
    [SerializeField] private float[] spawnPositionsX;
    private int previousLane;
    private float timePassed;
    private int spawnCount;

    void Awake()
    {
        timePassed = -spawnTime;
        spawnCount = 0;
        previousLane = -1;
    }

    void Update()
    {
        if (GameManager.instance.gameStarted)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= spawnTime) //modificare lo spawntime in base all'accelerazione
            {
                int i = ChooseLane();
                previousLane = i;
                float randomX = spawnPositionsX[i];
                i = Random.Range(0, carsList.cars.Length);
                GameObject car = Instantiate(carsList.cars[i].GetPrefab());
                car.transform.position = new Vector3(randomX, carsList.cars[i].GetHeight(), spawnPositionZ);
                if (spawnCount % 3 == 0 && spawnCount != 0)
                {
                    i = ChooseLane();
                    previousLane = i;
                    randomX = spawnPositionsX[i];
                    i = Random.Range(0, carsList.cars.Length);
                    GameObject secondCar = Instantiate(carsList.cars[i].GetPrefab());
                    secondCar.transform.position = new Vector3(randomX, carsList.cars[i].GetHeight(), spawnPositionZ);
                }
                spawnCount++;
                timePassed = 0;
            }
        }
    }

    private int ChooseLane()
    {
        int i = Random.Range(0, spawnPositionsX.Length);
        if (i == previousLane)
        {
            return ChooseLane();
        }
        return i;
    }
}
