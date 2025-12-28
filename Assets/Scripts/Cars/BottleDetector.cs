using System.Collections.Generic;
using UnityEngine;

public class BottleDetector : MonoBehaviour
{
    public float speedMultiplier = 1f;
    private Transform bottle = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Bottle"))
        {
            if (bottle == null)
                bottle = other.transform;
        }
    }

    private void Update()
    {
        if (bottle != null)
        {
            float distance = Vector3.Distance(transform.position, bottle.position);
            if (distance > 12)
                speedMultiplier = 1;
            else if (distance <= 12)
                speedMultiplier = 0.8f;
            else if (distance <= 11)
                speedMultiplier = 0.6f;
            else if (distance <= 10)
                speedMultiplier = 0.4f;
            else if (distance <= 9)
                speedMultiplier = 0.2f;
            else if (distance <= 8)
                speedMultiplier = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == bottle)
        {
            bottle = null;
        }
    }
}
