using UnityEngine;

public class CarCollision : MonoBehaviour
{
    [SerializeField] private float speedLoss;
    [SerializeField] private AudioClip[] collisionAudioClips;
    [SerializeField] private float[] collisionAudioVolumes;
    private DrinkSystem drinkSystem;
    private CameraMovement cameraHandler;

    private void Awake()
    {
        drinkSystem = transform.GetComponentInChildren<DrinkSystem>();
        cameraHandler = transform.GetComponentInChildren<CameraMovement>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Car"))
        {
            int i = Random.Range(0, collisionAudioClips.Length);
            SFXManager.instance.PlayClipWithRandomPitch(collisionAudioClips[i], collisionAudioVolumes[i]);
            CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
            ContactPoint contact = collision.GetContact(0);
            Vector3 contactPointLocal = transform.InverseTransformPoint(contact.point);
            if (contactPointLocal.x > 0)
                carMovement.StartSkidding(Direction.right);
            else
                carMovement.StartSkidding(Direction.left);
            drinkSystem.ShakeBeer();
            StartCoroutine(cameraHandler.HeadHitMovement());
            StartCoroutine(drinkSystem.LoseBeer(0.05f));
            if (!GameManager.instance.IsImmuneToDeceleration())
                GameManager.instance.UpdateAlcoolPower(-speedLoss);
        }
        else if (collision.transform.CompareTag("Bottle"))
        {
            collision.gameObject.GetComponent<BottleMovement>().Explode();
            //Ottieni alcoolPower e birra bevuta
        }
    }
}
