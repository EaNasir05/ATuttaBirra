using UnityEngine;

public class EntitiesSpawner : MonoBehaviour
{
    [SerializeField] private CarsList carsList;
    [SerializeField] private GameObject bottlePrefab;
    [SerializeField] private float spawnTime;
    [SerializeField] private float spawnPositionZ;
    [SerializeField] private float[] spawnPositionsX;
    private int previousLane;
    private float timePassed;
    private int spawnCount;
    private int spawnCarCount;

    void Awake()
    {
        timePassed = 0;
        spawnCount = 0;
        spawnCarCount = 0;
        previousLane = -1;
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= spawnTime)
        {
            if (spawnCount % 5 == 0)
            {
                int direction = Random.Range(0, 2);
                float speed = Random.Range(0, 3);
                GameObject bottle = Instantiate(bottlePrefab);
                float x = 19;
                if (direction == 0)
                {
                    x = -19;
                    bottle.GetComponent<BottleMovement>().moveDirection = new Vector2(1, 0);
                }
                bottle.transform.position = new Vector3(x, 1, spawnPositionZ);
                switch (speed)
                {
                    case 0:
                        bottle.GetComponent<BottleMovement>().moveSpeed -= 1f;
                        break;
                    case 1:
                        bottle.GetComponent<BottleMovement>().moveSpeed += 1f;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                int i = ChooseLane();
                previousLane = i;
                float randomX = spawnPositionsX[i];
                i = Random.Range(0, carsList.cars.Length);
                GameObject car = Instantiate(carsList.cars[i].GetPrefab());
                car.transform.position = new Vector3(randomX, carsList.cars[i].GetHeight(), spawnPositionZ);
                car.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                if (spawnCarCount % 3 == 0)
                {
                    i = ChooseLane();
                    previousLane = i;
                    randomX = spawnPositionsX[i];
                    i = Random.Range(0, carsList.cars.Length);
                    GameObject secondCar = Instantiate(carsList.cars[i].GetPrefab());
                    secondCar.transform.position = new Vector3(randomX, carsList.cars[i].GetHeight(), spawnPositionZ);
                    secondCar.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                spawnCarCount++;
            }
            spawnCount++;
            timePassed = 0;
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
