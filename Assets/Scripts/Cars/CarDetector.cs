using System.Collections;
using UnityEngine;

public class CarDetector : MonoBehaviour
{
    public float speed = -9999999;
    private CarMovement movementSystem;
    private CarMovement nearestCar = null;
    private bool approachingACar;
    private bool slowingDown = false;
    private bool followingTheCar = false;


    private void Awake()
    {
        movementSystem = GetComponentInParent<CarMovement>();
        approachingACar = false;
    }

    private void Update()
    {
        if (!approachingACar)
            speed = -9999999;
        else
        {
            float baseSpeed = -movementSystem.GetBaseSpeed() + (-movementSystem.GetAccelerationMultiplier() * GameManager.instance.GetAlcoolPower());
            if (baseSpeed < nearestCar.currentSpeed && !slowingDown)
            {
                if (!followingTheCar)
                    StartCoroutine(SlowDown(baseSpeed));
                else
                    speed = nearestCar != null ? nearestCar.currentSpeed : -9999999;
            }
        }
    }

    private IEnumerator SlowDown(float baseSpeed)
    {
        slowingDown = true;
        float elapsed = 0;
        float duration = Mathf.Clamp(1 / GameManager.instance.GetAlcoolPower(), 0.2f, 1);
        float targetSpeed = nearestCar.currentSpeed;
        while (elapsed < duration && slowingDown)
        {
            elapsed += Time.deltaTime;
            speed = Mathf.Lerp(baseSpeed, targetSpeed, elapsed / duration);
            yield return null;
        }
        followingTheCar = true;
        slowingDown = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            if (nearestCar == null)
            {
                nearestCar = other.GetComponent<CarMovement>();
                approachingACar = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            if (nearestCar == other.GetComponent<CarMovement>())
            {
                nearestCar = null;
                approachingACar = false;
                speed = -9999999;
                slowingDown = false;
                followingTheCar = false;
            }
        }
    }
}
