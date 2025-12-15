using UnityEngine;

public class BottigliaIntera : MonoBehaviour
{
    [SerializeField] GameObject bottigliaRottaPrefab;  
    [SerializeField] float tempoPrimaDistruggere = 60f; 

    
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.relativeVelocity.magnitude > 1f)  
        {
            Esplodi();
        }
    }

    
    private void Esplodi()
    {
        
        GameObject bottigliaRotta = Instantiate(bottigliaRottaPrefab, transform.position, transform.rotation);

        Destroy(gameObject);

        
        Destroy(bottigliaRotta, tempoPrimaDistruggere);
    }
}
