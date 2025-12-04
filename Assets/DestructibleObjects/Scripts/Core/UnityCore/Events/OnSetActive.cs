using UnityEngine;
using UnityEngine.Events;

public class OnSetActive : MonoBehaviour
{
    public UnityEvent OnActive;

    private void OnEnable() {
        OnActive?.Invoke();
    }
}
