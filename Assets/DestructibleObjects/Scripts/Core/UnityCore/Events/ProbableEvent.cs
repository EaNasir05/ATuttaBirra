using UnityEngine;
using UnityEngine.Events;

public class ProbableEvent : MonoBehaviour
{   
    [SerializeField]
    [Range(0f,1f)] private float _probability;

    public UnityEvent OnEventTrue;
    public UnityEvent OnEventFalse;

    public void StartProbableEvent()
    {
        if(Random.Range(0f,1f) <= _probability)
        {
            OnEventTrue?.Invoke();
        }
        else
        {
            OnEventFalse?.Invoke();
        }
    }

}
