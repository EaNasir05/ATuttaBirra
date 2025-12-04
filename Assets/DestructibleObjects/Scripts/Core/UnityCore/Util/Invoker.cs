using System.Collections;
using UnityEngine;

public static class Invoker
{
    public static void Invoke(this MonoBehaviour mb, System.Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }
 
    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        f();
    }

    public static void InvokeScaledDeltaTime(this MonoBehaviour mb, System.Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutineScaledDeltaTime(f, delay));
    }
 
    private static IEnumerator InvokeRoutineScaledDeltaTime(System.Action f, float delay)
    {   
        while(Time.timeScale == 0)
        {
            yield return null;
        }
        float delayInScale = delay /= Mathf.Clamp(Time.timeScale,0.01f,Mathf.Infinity);
        float timeElapsed = 0f;
        while (timeElapsed < delayInScale)
        {
            while(Time.timeScale == 0)
            {
                yield return null;
            }
            timeElapsed += Time.deltaTime / Time.timeScale;
            yield return null;
        }
        f();
    }

    public static void InvokeRepeatingScaledDeltaTime(this MonoBehaviour mb, System.Action f, float delay)
    {
        mb.StartCoroutine(InvokeRepeatingRoutineScaledDeltaTime(mb, f, delay));
    }

    private static IEnumerator InvokeRepeatingRoutineScaledDeltaTime(this MonoBehaviour mb, System.Action f, float delay)
    {
        while(Time.timeScale == 0)
        {
            yield return null;
        }
        float delayInScale = delay /= Mathf.Clamp(Time.timeScale,0.01f,Mathf.Infinity);
        float timeElapsed = 0f;
        while (timeElapsed < delayInScale)
        {
            if(Time.timeScale > 0)
                timeElapsed += Time.deltaTime / Time.timeScale;
            yield return null;
        }
        f();
        mb.StartCoroutine(InvokeRepeatingRoutineScaledDeltaTime(mb, f, delay));
    }

    public static void InvokeRepeating(this MonoBehaviour mb, System.Action f, float delay)
    {
        mb.StartCoroutine(InvokeRepeatingRoutine(mb, f, delay));
    }

    private static IEnumerator InvokeRepeatingRoutine(this MonoBehaviour mb, System.Action f, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        f();
        mb.StartCoroutine(InvokeRepeatingRoutine(mb, f, delay));
    }
}
