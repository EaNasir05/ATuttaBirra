using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesSpawner : MonoBehaviour
{
    [SerializeField] private CarsList carsList;
    [SerializeField] private CarsBlocks carsBlocks;
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

    void Awake()
    {
        timePassed = -spawnTime;
        spawnTime = startingSpawnTime;
        littleCars = carsList.GetLittleCars();
        bigCars = carsList.GetBigCars();
        blocks = carsBlocks.blocks;
        selectedBlock = blocks[Random.Range(0, 6)];
    }

    void Update()
    {
        if (GameManager.instance.gameStarted)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= spawnTime)
            {
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
                    spawnedCar.transform.position = new Vector3(posX, car.GetHeight(), spawnPositionZ + selectedBlock.carsPositionZ[i]);
                }
                int newBlock = selectedBlock.possibleNextBlocksIndexes[Random.Range(0, selectedBlock.possibleNextBlocksIndexes.Length)];
                Debug.Log(newBlock);
                selectedBlock = blocks[newBlock];
                timePassed = 0;
            }
        }
    }

    /*
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
    */

public void UpdateSpawnTime()
    {
        spawnTime = Mathf.Clamp(spawnTime - ((int)((GameManager.instance.GetTotalBeerConsumed() - 1) / 5) * spawnTimeReduction), minSpawnTime, startingSpawnTime);
    }
}
