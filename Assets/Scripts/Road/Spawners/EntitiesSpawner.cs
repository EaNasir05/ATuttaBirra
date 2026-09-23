using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesSpawner : MonoBehaviour
{
    [System.Serializable]
    public class CarsBlocksIndexes
    {
        public List<int> blocks = new();
    }

    [SerializeField] private CarsList carsList;
    [SerializeField] private CarsBlocks carsBlocks;
    [SerializeField] private List<CarsBlocksIndexes> startingBlocks = new();
    [SerializeField] private float startingSpawnTime;
    [SerializeField] private float spawnPositionZ;
    [SerializeField] private float leftLaneEquivalentPositionX;
    [SerializeField] private float centerLaneEquivalentPositionX;
    [SerializeField] private float rightLaneEquivalentPositionX;
    [SerializeField] private float extendedLeftLaneEquivalentPositionX;
    [SerializeField] private float extendedRightLaneEquivalentPositionX;
    private List<Car> littleCars;
    private List<Car> bigCars;
    private CarsBlock[] blocks;
    private float spawnTime;
    private float timePassed;
    private CarsBlock selectedBlock;
    private int[] blocksBehindObstacle;
    private bool firstBlock = true;
    private bool spawnAfterObstacle = false;
    private int currentBiome = 0;
    private bool obstacleSpawned = false;
    private RoadLane laneBlocked;
    private readonly float roadLenght = 258;
    private readonly float carsAvarageSpeed = 35;
    private readonly float obstaclesAvarageSpeed = 15;

    void Awake()
    {
        timePassed = -spawnTime;
        spawnTime = startingSpawnTime;
        littleCars = carsList.GetLittleCars(0);
        bigCars = carsList.GetBigCars(0);
        blocks = carsBlocks.blocks;
    }

    void Update()
    {
        if (GameManager.instance.gameStarted)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= spawnTime)
            {
                if (spawnAfterObstacle)
                {
                    if (blocksBehindObstacle != null && blocksBehindObstacle.Length > 0)
                        selectedBlock = blocks[blocksBehindObstacle[Random.Range(0, blocksBehindObstacle.Length)]];
                    spawnAfterObstacle = false;
                }
                if (firstBlock)
                {
                    selectedBlock = blocks[startingBlocks[currentBiome].blocks[Random.Range(0, startingBlocks[currentBiome].blocks.Count)]];
                    firstBlock = false;
                }
                if (selectedBlock.isThereAnObstacle)
                {
                    if (obstacleSpawned)
                    {
                        Debug.LogError("Può essere bloccata solo una corsia alla volta");
                    }
                    else
                    {
                        RoadObstacle obstacle = selectedBlock.obstacle;
                        RoadLane lane = obstacle.lane;
                        float posX = 0;
                        switch (lane)
                        {
                            case RoadLane.left:
                                posX = leftLaneEquivalentPositionX;
                                break;
                            case RoadLane.center:
                                posX = centerLaneEquivalentPositionX;
                                lane = RoadLane.left;
                                Debug.LogError("Non può esistere un ostacolo nella corsia centrale");
                                break;
                            case RoadLane.right:
                                posX = rightLaneEquivalentPositionX;
                                break;
                            case RoadLane.extendedLeft:
                                posX = extendedLeftLaneEquivalentPositionX;
                                break;
                            case RoadLane.extendedRight:
                                posX = extendedRightLaneEquivalentPositionX;
                                break;
                        }
                        laneBlocked = lane;
                        blocksBehindObstacle = obstacle.possibleBlocksBehindThisObstacle;
                        GameObject spawnedObstacle = Instantiate(obstacle.prefab);
                        spawnedObstacle.transform.position = new Vector3(posX, obstacle.positionY, obstacle.positionZ);
                        obstacleSpawned = true;
                    }
                }
                for (int i = 0; i < selectedBlock.cars.Length; i++)
                {
                    float posX = 0;
                    Car car;
                    RoadLane lane = selectedBlock.cars[i].lane;
                    switch (lane)
                    {
                        case RoadLane.left:
                            if (obstacleSpawned && lane == laneBlocked)
                            {
                                Debug.LogError("Non può essere istanziata una macchina su una corsia bloccata");
                                posX = 900;
                            }
                            else
                                posX = leftLaneEquivalentPositionX;
                            break;
                        case RoadLane.center:
                            if (obstacleSpawned && lane == laneBlocked)
                            {
                                Debug.LogError("Non può essere istanziata una macchina su una corsia bloccata");
                                posX = 900;
                            }
                            else
                                posX = centerLaneEquivalentPositionX;
                            break;
                        case RoadLane.right:
                            if (obstacleSpawned && lane == laneBlocked)
                            {
                                Debug.LogError("Non può essere istanziata una macchina su una corsia bloccata");
                                posX = 900;
                            }
                            else
                                posX = rightLaneEquivalentPositionX;
                            break;
                        case RoadLane.extendedLeft:
                            if (obstacleSpawned && lane == laneBlocked)
                            {
                                Debug.LogError("Non può essere istanziata una macchina su una corsia bloccata");
                                posX = 900;
                            }
                            else
                                posX = extendedLeftLaneEquivalentPositionX;
                            break;
                        case RoadLane.extendedRight:
                            if (obstacleSpawned && lane == laneBlocked)
                            {
                                Debug.LogError("Non può essere istanziata una macchina su una corsia bloccata");
                                posX = 900;
                            }
                            else
                                posX = extendedRightLaneEquivalentPositionX;
                            break;
                    }
                    if (selectedBlock.cars[i].mirage)
                        Debug.LogWarning("Ancora non è implementata la possibilità di avere miraggi");
                    if (selectedBlock.cars[i].big)
                        car = bigCars[Random.Range(0, bigCars.Count)];
                    else
                        car = littleCars[Random.Range(0, littleCars.Count)];
                    GameObject spawnedCar = Instantiate(car.GetPrefab());
                    spawnedCar.transform.position = new Vector3(posX + selectedBlock.cars[i].positionX, car.GetHeight(), spawnPositionZ + selectedBlock.cars[i].positionZ);
                }
                int newBlock = selectedBlock.possibleNextBlocksIndexes[Random.Range(0, selectedBlock.possibleNextBlocksIndexes.Length)];

                /*
                count++;
                if (count % 3 == 0 && count != 0)
                    newBlock = 18;
                */
                spawnTime = selectedBlock.spawnDelay;
                selectedBlock = blocks[newBlock];
                timePassed = 0;
            }
        }
    }

    public void IncreaseBiomeIndex()
    {
        currentBiome++;
        littleCars = carsList.GetLittleCars(currentBiome);
        bigCars = carsList.GetBigCars(currentBiome);
        firstBlock = true;
    }

    public void ClearLane()
    {
        float alcoolPower = GameManager.instance.GetAlcoolPower();
        float delay = (roadLenght / (obstaclesAvarageSpeed * alcoolPower)) - (roadLenght / (carsAvarageSpeed * alcoolPower));
        StartCoroutine(SpawnBlocksBehindObstacles(delay));
    }

    private IEnumerator SpawnBlocksBehindObstacles(float totalDelay, float delay)
    {
        yield return new WaitForSeconds(delay);
        float alcoolPower = GameManager.instance.GetAlcoolPower();
        float newDelay = (roadLenght / (obstaclesAvarageSpeed * alcoolPower)) - (roadLenght / (carsAvarageSpeed * alcoolPower));
        if (newDelay > totalDelay)
        {
            StartCoroutine(SpawnBlocksBehindObstacles(totalDelay + newDelay, newDelay - totalDelay));
        }
        else
        {
            obstacleSpawned = false;
            spawnAfterObstacle = true;
        }
    }

    private IEnumerator SpawnBlocksBehindObstacles(float delay)
    {
        yield return new WaitForSeconds(delay);
        obstacleSpawned = false;
        spawnAfterObstacle = true;
    }
}
