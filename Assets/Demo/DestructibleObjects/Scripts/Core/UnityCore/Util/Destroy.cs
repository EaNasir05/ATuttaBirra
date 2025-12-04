using UnityEngine;

public class Destroy : MonoBehaviour
{
    public void DestroyItself()
    {
        Destroy(gameObject);
    }

    public void DestroyItselfDelay(float delay)
    {
        Destroy(gameObject,delay);
    }

    public void DestroyOther(GameObject other)
    {
        Destroy(other);
    }
}
