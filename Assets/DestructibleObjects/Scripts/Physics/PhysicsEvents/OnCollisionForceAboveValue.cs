using UnityEngine;
using UnityEngine.Events;

public class OnCollisionForceAboveValue : MonoBehaviour, ICollision
{
    [SerializeField] private FloatVariable _value;
    [SerializeField] private FloatVariable _cooldown;

    public UnityEvent OnCollision;
    public UnityEvent OnCollisionWithNoRigidbody;

    private bool _collisionEnabled;

    public Collision Collision { get => _collision; set => _collision = value; }

    private Collision _collision;

    private void Awake() => _collisionEnabled = true;

    private void OnCollisionEnter(Collision collision)
    {
        _collision = collision;
        if(_collisionEnabled && collision.relativeVelocity.magnitude >= _value.Value)
        {
            _collisionEnabled = false;
            this.Invoke(() => _collisionEnabled = true,_cooldown.Value);
            OnCollision?.Invoke();
            if(collision.rigidbody == null)
                OnCollisionWithNoRigidbody?.Invoke();
        }
    }
}