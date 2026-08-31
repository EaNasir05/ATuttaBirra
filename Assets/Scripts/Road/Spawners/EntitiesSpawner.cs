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
    [SerializeField] private RoadObstacles obstaclesList;
    [SerializeField] private List<CarsBlocksIndexes> startingBlocks = new();
    [SerializeField] private float startingSpawnTime;
    [SerializeField] private float spawnPositionZ;
    [SerializeField] private float[] spawnPositionsX;
    private List<Car> littleCars;
    private List<Car> bigCars;
    private CarsBlock[] blocks;
    private List<RoadObstacle> obstacles;
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
        obstacles = obstaclesList.GetObstaclesFromBiome(0);
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
                        RoadLane lane = selectedBlock.obstacleLane;
                        float posX = 0;
                        switch (lane)
                        {
                            case RoadLane.left:
                                posX = spawnPositionsX[0];
                                break;
                            case RoadLane.center:
                                posX = spawnPositionsX[0];
                                lane = RoadLane.left;
                                Debug.LogError("Non può esistere un ostacolo nella corsia centrale");
                                break;
                            case RoadLane.right:
                                posX = spawnPositionsX[2];
                                break;
                        }
                        laneBlocked = lane;
                        blocksBehindObstacle = selectedBlock.possibleBlocksBehindThisObstacle;
                        List<RoadObstacle> selectedObstacles = RoadObstacles.SelectObstaclesByLane(obstacles, lane);
                        RoadObstacle obstacle = selectedObstacles[Random.Range(0, selectedObstacles.Count)];
                        GameObject spawnedObstacle = Instantiate(obstacle.prefab);
                        spawnedObstacle.transform.position = new Vector3(posX, obstacle.positionY, obstacle.positionZ);
                        obstacleSpawned = true;
                    }
                }
                for (int i = 0; i < selectedBlock.carsPositionZ.Length; i++)
                {
                    float posX = 0;
                    Car car;
                    RoadLane lane = selectedBlock.carsLane[i];
                    switch (lane)
                    {
                        case RoadLane.left:
                            if (obstacleSpawned && lane == laneBlocked)
                            {
                                Debug.LogError("Non può essere istanziata una macchina su una corsia bloccata");
                                posX = 900;
                            }
                            else
                                posX = spawnPositionsX[0];
                            break;
                        case RoadLane.center:
                            if (obstacleSpawned && lane == laneBlocked)
                            {
                                Debug.LogError("Non può essere istanziata una macchina su una corsia bloccata");
                                posX = 900;
                            }
                            else
                                posX = spawnPositionsX[1];
                            break;
                        case RoadLane.right:
                            if (obstacleSpawned && lane == laneBlocked)
                            {
                                Debug.LogError("Non può essere istanziata una macchina su una corsia bloccata");
                                posX = 900;
                            }
                            else
                                posX = spawnPositionsX[2];
                            break;
                    }
                    if (selectedBlock.bigCars[i])
                        car = bigCars[Random.Range(0, bigCars.Count)];
                    else
                        car = littleCars[Random.Range(0, littleCars.Count)];
                    GameObject spawnedCar = Instantiate(car.GetPrefab());
                    spawnedCar.transform.position = new Vector3(posX + (i < selectedBlock.carsPositionX.Length ? selectedBlock.carsPositionX[i] : 0), car.GetHeight(), spawnPositionZ + selectedBlock.carsPositionZ[i]);
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
        obstacles = obstaclesList.GetObstaclesFromBiome(currentBiome);
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
