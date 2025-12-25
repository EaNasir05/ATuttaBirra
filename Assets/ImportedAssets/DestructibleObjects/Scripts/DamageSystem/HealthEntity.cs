using UnityEngine;
using UnityEngine.Events;

public class HealthEntity : MonoBehaviour, IDamageTaker
{
    [SerializeField] private FloatVariable _maxValue;
    [SerializeField] private float _startingValue;
    
    public bool HasBeenDestructed { get; set; }
    public bool LastDamageKilledIt { get; set; }
    
    public UnityEvent OnGetDamage;
    public UnityEvent OnGetHeal;
    public UnityEvent OnDestroy;

    public Amount Health;

    private void Awake()
    {
        Init();
    }

    public void TakeDamage(float damage)
    {
        if(HasBeenDestructed)
        {
            LastDamageKilledIt = false;
            return;
        }
        Health.Substract(damage);
        if(Health.Value <= 0 )
        {
            Health.SetAmount(0);
            if(!HasBeenDestructed)
            {
                Destroy();
            }
        }
    }

    public void Destroy()
    {
        if(Health == null)
        {
            Init();
        }
        Health.SetAmount(0);
        HasBeenDestructed = true;
        LastDamageKilledIt = true;
        OnDestroy?.Invoke();
    }

    void Init()
    {
        Health = new Amount(_startingValue,0,_maxValue.Value);
        Health.OnSubstractToTheAmount = () => OnGetDamage?.Invoke();
        Health.OnAddToTheAmount = () => OnGetHeal?.Invoke();
        LastDamageKilledIt = false;
    }
}
