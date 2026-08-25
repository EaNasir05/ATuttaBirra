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
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float spawnTimeReduction;
    [SerializeField] private float spawnPositionZ;
    [SerializeField] private float[] spawnPositionsX;
    private List<Car> littleCars;
    private List<Car> bigCars;
    private CarsBlock[] blocks;
    private float spawnTime;
    private float timePassed;
    private CarsBlock selectedBlock;
    private int count = 0;
    private bool firstBlock = true;
    private int currentBiome = 0;

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
                if (firstBlock)
                {
                    selectedBlock = blocks[startingBlocks[currentBiome].blocks[Random.Range(0, startingBlocks[currentBiome].blocks.Count)]];
                    firstBlock = false;
                }
                for (int i = 0; i < selectedBlock.carsPositionZ.Length; i++)
                {
                    float posX = 0;
                    Car car;
                    switch (selectedBlock.carsLane[i])
                    {
                        case RoadLane.left:
                            posX = spawnPositionsX[0];
                            break;
                        case RoadLane.center:
                            posX = spawnPositionsX[1];
                            break;
                        case RoadLane.right:
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

    public void UpdateSpawnTime()
    {
        spawnTime = Mathf.Clamp(spawnTime - ((int)((GameManager.instance.GetTotalBeerConsumed() - 1) / 5) * spawnTimeReduction), minSpawnTime, startingSpawnTime);
    }
}
