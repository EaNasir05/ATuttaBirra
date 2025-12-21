using UnityEngine;

public class CarCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Car"))
        {
            CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
            ContactPoint contact = collision.GetContact(0);
            Vector3 contactPointLocal = transform.InverseTransformPoint(contact.point);
            if (contactPointLocal.x > 0)
            {
                Debug.Log("DESTRA");
            }
            else
            {
                Debug.Log("SINISTRA");
            }
        }
    }
}
