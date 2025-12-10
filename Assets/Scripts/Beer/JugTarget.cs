using UnityEngine;

public class JugTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Beer Jug"))
        {
            Debug.Log("BEVI!");
            other.GetComponent<DrinkSystem>().StartDrinking();
        }
    }
}
