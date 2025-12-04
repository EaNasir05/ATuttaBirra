using UnityEngine;
using UnityEngine.Events;

public class EventAfterTime : MonoBehaviour
{
    public UnityEvent Event;

    public void RaiseEvent(float delay) => this.InvokeScaledDeltaTime(() => Event?.Invoke(),delay);

    public void RaiseEventInLoop(float delay) => this.InvokeRepeatingScaledDeltaTime(() => Event?.Invoke(),delay);
    
    public void RaiseEventInLoopImmediately(float delay)
    {
        Event?.Invoke();
        RaiseEventInLoop(delay);
    }

    public void UnscaledRaiseEvent(float delay) => this.Invoke(() => Event?.Invoke(),delay);

    public void UnscaledRaiseEventInLoop(float delay) => this.InvokeRepeating(() => Event?.Invoke(),delay);
    
    public void UnscaledRaiseEventInLoopImmediately(float delay)
    {
        Event?.Invoke();
        UnscaledRaiseEventInLoop(delay);
    }

    public void StopInvoke() => StopAllCoroutines();
    public void StopLoop() => StopAllCoroutines();
}
