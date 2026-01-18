using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
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
    private int count = 0;

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
                    spawnedCar.transform.position = new Vector3(posX + (i < selectedBlock.carsPositionX.Length ? selectedBlock.carsPositionX[i] : 0), car.GetHeight(), spawnPositionZ + selectedBlock.carsPositionZ[i]);
                }
                int newBlock = selectedBlock.possibleNextBlocksIndexes[Random.Range(0, selectedBlock.possibleNextBlocksIndexes.Length)];

                /*
                count++;
                if (count % 3 == 0 && count != 0)
                    newBlock = INDICE_BLOCCO;
                */

                selectedBlock = blocks[newBlock];
                timePassed = 0;
            }
        }
    }

    public void UpdateSpawnTime()
    {
        spawnTime = Mathf.Clamp(spawnTime - ((int)((GameManager.instance.GetTotalBeerConsumed() - 1) / 5) * spawnTimeReduction), minSpawnTime, startingSpawnTime);
    }
}
