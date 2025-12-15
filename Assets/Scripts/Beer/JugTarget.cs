using UnityEngine;

public class JugTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Beer Jug"))
        {
            DrinkSystem drinkSystem = other.GetComponent<DrinkSystem>();
            if (drinkSystem.IsMoving())
            {
                drinkSystem.StartDrinking();
            }
        }
    }
}
