using UnityEngine;

public class CarCollision : MonoBehaviour
{
    private DrinkSystem drinkSystem;

    private void Awake()
    {
        drinkSystem = transform.GetComponentInChildren<DrinkSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Car"))
        {
            CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
            ContactPoint contact = collision.GetContact(0);
            Vector3 contactPointLocal = transform.InverseTransformPoint(contact.point);
            if (contactPointLocal.x > 0)
            {
                //DESTRA
            }
            else
            {
                //SINISTRA
            }
            //StartCoroutine(drinkSystem.LoseBeer(0.5f));
        }
        else if (collision.transform.CompareTag("Bottle"))
        {
            collision.gameObject.GetComponent<BottleMovement>().Explode();
            //StartCoroutine(drinkSystem.GainBeer(0.5f));
        }
    }
}
