using UnityEngine;

public class DrinkObserver : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private Liquid beer;
    [SerializeField] private PoliceChaseSystem policeChase;

    [Header("Rilevamento sorso")]
    [Tooltip("Quanto deve diminuire il fill per contare come sorso")]
    [SerializeField] private float sipThreshold = 0.03f;

    [Tooltip("Tempo minimo tra due sorsi (evita spam)")]
    [SerializeField] private float sipCooldown = 0.5f;

    private float lastFill;
    private float lastSipTime;

    private void Start()
    {
        if (beer == null)
        {
            Debug.LogError("DrinkObserver: beer non assegnata");
            enabled = false;
            return;
        }

        lastFill = beer.fillAmount;
        lastSipTime = -sipCooldown;
    }

    private void Update()
    {
        float currentFill = beer.fillAmount;

        
        if (currentFill < lastFill)
        {
            float delta = lastFill - currentFill;

            
            if (delta >= sipThreshold && Time.time - lastSipTime >= sipCooldown)
            {
                lastSipTime = Time.time;
                //policeChase.OnPlayerDrink();
            }
        }

        lastFill = currentFill;
    }
}
