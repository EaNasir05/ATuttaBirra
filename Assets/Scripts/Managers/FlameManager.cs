using UnityEngine;

public class FlameManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem Flame;
    [SerializeField] private float minConsume = 1f;
    [SerializeField] private float maxConsume = 11f;
    [SerializeField] private float minScale = 0.5f; // fattore minima scala (0.5 = metà dimensione)
    [SerializeField] private float maxScale = 2f;   // fattore massima scala (2 = doppia dimensione)

    private float baseStartSize = 1f;
    private float baseEmissionRate = 10f;

    void Start()
    {
        if (Flame == null) Flame = GetComponent<ParticleSystem>();

        if (Flame == null) return;

        var main = Flame.main;
        var startCurve = main.startSize;
        // prendo valore costante o la media min/max
        if (startCurve.mode == ParticleSystemCurveMode.Constant)
            baseStartSize = startCurve.constant;
        else
            baseStartSize = (startCurve.constantMin + startCurve.constantMax) * 0.5f;

        var emission = Flame.emission;
        var rateCurve = emission.rateOverTime;
        if (rateCurve.mode == ParticleSystemCurveMode.Constant)
            baseEmissionRate = rateCurve.constant;
        else
            baseEmissionRate = (rateCurve.constantMin + rateCurve.constantMax) * 0.5f;
    }

    void Update()
    {
        if (Flame == null || GameManager.instance == null) return;

        float consumed = GameManager.instance.GetTotalBeerConsumed();
        float t = Mathf.InverseLerp(minConsume, maxConsume, consumed); // 0..1
        float scale = Mathf.Lerp(minScale, maxScale, t);               // fattore di scala

        ApplyScale(scale);
    }

    private void ApplyScale(float scale)
    {
        var main = Flame.main;
        main.startSize = new ParticleSystem.MinMaxCurve(baseStartSize * scale);

        var emission = Flame.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(baseEmissionRate * scale);
    }
}