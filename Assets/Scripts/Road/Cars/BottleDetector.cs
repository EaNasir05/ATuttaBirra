using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BottleDetector : MonoBehaviour
{
    public float speedMultiplier = 1f;
    private List<Transform> bottles = new List<Transform>();
    private Transform nearestBottle;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTRATA");
        if (other.transform.CompareTag("Bottle"))
        {
            Debug.Log("ENTRATA BOTTIGLIA");
            if (bottles.Count == 0)
                nearestBottle = other.transform;
            if (!bottles.Contains(other.transform))
                bottles.Add(other.transform);
        }
    }

    private void Update()
    {
        foreach (Transform bottle in bottles)
        {
            float distance = Vector3.Distance(transform.position, bottle.position);
            Debug.Log(distance);
            if (distance > 12)
            {
                if (nearestBottle == bottle)
                {
                    speedMultiplier = 1;
                }
            }
            else if (distance <= 12)
            {
                if (nearestBottle == bottle)
                {
                    speedMultiplier = 0.8f;
                }
                else if (distance < Vector3.Distance(transform.position, nearestBottle.position))
                {
                    nearestBottle = bottle;
                    speedMultiplier = 0.8f;
                }
            }
            else if (distance <= 10)
            {
                if (nearestBottle == bottle)
                {
                    speedMultiplier = 0.6f;
                }
                else if (distance < Vector3.Distance(transform.position, nearestBottle.position))
                {
                    nearestBottle = bottle;
                    speedMultiplier = 0.6f;
                }
            }
            else if (distance <= 8)
            {
                if (nearestBottle == bottle)
                {
                    speedMultiplier = 0.4f;
                }
                else if (distance < Vector3.Distance(transform.position, nearestBottle.position))
                {
                    nearestBottle = bottle;
                    speedMultiplier = 0.4f;
                }
            }
            else if (distance <= 6)
            {
                if (nearestBottle == bottle)
                {
                    speedMultiplier = 0.2f;
                }
                else if (distance < Vector3.Distance(transform.position, nearestBottle.position))
                {
                    nearestBottle = bottle;
                    speedMultiplier = 0.2f;
                }
            }
            else if (distance <= 4)
            {
                if (nearestBottle == bottle)
                {
                    speedMultiplier = 0;
                }
                else if (distance < Vector3.Distance(transform.position, nearestBottle.position))
                {
                    nearestBottle = bottle;
                    speedMultiplier = 0;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Bottle"))
        {
            bottles.Remove(other.transform);
            if (bottles.Count > 0 && other.transform == nearestBottle)
                nearestBottle = bottles[0];
        }
    }
}
