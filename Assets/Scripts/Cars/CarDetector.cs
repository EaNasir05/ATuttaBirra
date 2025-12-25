using UnityEngine;

public class CarDetector : MonoBehaviour
{
    public float speed = -9999999;
    private CarMovement movementSystem;
    private CarMovement nearestCar = null;
    private bool approachingACar;


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
            if (baseSpeed > nearestCar.currentSpeed)
            {
                float distance = Vector3.Distance(movementSystem.transform.position, nearestCar.transform.position);
                if (distance > 12)
                    speed = -9999999;
                else if (distance <= 12)
                    speed = Mathf.Lerp(baseSpeed, nearestCar.currentSpeed, 0.2f);
                else if (distance <= 11)
                    speed = Mathf.Lerp(baseSpeed, nearestCar.currentSpeed, 0.4f);
                else if (distance <= 10)
                    speed = Mathf.Lerp(baseSpeed, nearestCar.currentSpeed, 0.6f);
                else if (distance <= 9)
                    speed = Mathf.Lerp(baseSpeed, nearestCar.currentSpeed, 0.8f);
                else if (distance <= 8)
                    speed = nearestCar.currentSpeed;
            }
        }
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
            }
        }
    }
}
