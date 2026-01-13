using UnityEngine;
using static Unity.VisualScripting.Member;

[RequireComponent(typeof(LineRenderer))]
public class LiquidStreamToggle : MonoBehaviour
{
    [Header("Flow Settings")]
    public Transform startPoint;
    public float maxDistance = 5f;
    public int segments = 20;
    public float gravityCurve = 0.1f;
    public float flowSpeed = 5f;

    [Header("Collision and DrinkSystem")]
    public LayerMask collisionMask;
    public DrinkSystem drinkSystem;
    public float beerGainRate;

    [Header("Optional")]
    public ParticleSystem splashParticles;

    
    [Header("Enough Beer")]
    public GameObject EnoughBeer;

    private LineRenderer line;
    private bool isFlowing = false;
    private float currentLength = 0f;
    private bool wasFillingTheJug = false;
    private bool isFillingTheJug = false;
    private AudioSource audioSource;
    private AudioSource loopAudioSource;
    private float startingLoopSeconds;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments;
        line.enabled = false;
        audioSource = GetComponent<AudioSource>();
        loopAudioSource = startPoint.GetComponent<AudioSource>();
        startingLoopSeconds = loopAudioSource.clip.length * 0.25f;

        if (splashParticles != null)
            splashParticles.Stop();
        
        if (EnoughBeer != null)
            EnoughBeer.SetActive(false);
    }

    void Update()
    {
        if (isFlowing)
        {
            currentLength += flowSpeed * Time.deltaTime;
            DrawStream();
        }
        else
            isFillingTheJug = false;

        CheckFillingTheJug();

        
        CheckEnoughBeer();

        if (loopAudioSource.isPlaying && loopAudioSource.time >= loopAudioSource.clip.length)
        {
            loopAudioSource.time = startingLoopSeconds;
        }
    }

   
    private void CheckEnoughBeer()
    {
        if (EnoughBeer == null || drinkSystem == null)
            return;
        
        bool overLimit = drinkSystem.GetBeerFill() <= drinkSystem.GetMinFill();

        if (isFillingTheJug && overLimit)
        {
            if (!EnoughBeer.activeSelf)
                EnoughBeer.SetActive(true);
        }
        else
        {
            if (EnoughBeer.activeSelf)
                EnoughBeer.SetActive(false);
        }
    }

    private void CheckFillingTheJug()
    {
        if (!wasFillingTheJug && isFillingTheJug)
        {
            drinkSystem.receivingBeer = true;
            wasFillingTheJug = true;
            float perc = 1 - ((drinkSystem.GetBeerFill() - drinkSystem.GetMinFill()) / (drinkSystem.GetMaxFill() - drinkSystem.GetMinFill()));
            audioSource.Play();
            audioSource.time = perc * audioSource.clip.length;
        }
        else if (wasFillingTheJug && !isFillingTheJug)
        {
            drinkSystem.receivingBeer = false;
            wasFillingTheJug = false;
            audioSource.Stop();
        }
        if (drinkSystem.beerOverflowing)
            audioSource.Stop();
    }

    public void SetFlow(bool active)
    {
        if (isFlowing == active)
            return;

        isFlowing = active;
        line.enabled = active;

        if (!active)
        {
            currentLength = 0f;
            if (splashParticles != null)
                splashParticles.Stop();
        }
    }

    public void StartLoopClip()
    {
        if (!loopAudioSource.isPlaying)
            loopAudioSource.Play();
    }

    public void StopLoopClip()
    {
        if (loopAudioSource.isPlaying)
            loopAudioSource.Stop();
    }

    void DrawStream()
    {
        Vector3 start = startPoint.position;

        float targetLength = maxDistance;
        Vector3 end = start + Vector3.down * maxDistance;
        isFillingTheJug = false;

        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, maxDistance, collisionMask))
        {
            targetLength = hit.distance;
            end = hit.point;

            if (hit.collider.CompareTag("StreamTarget"))
            {
                drinkSystem.GainBeer(beerGainRate * Time.deltaTime);
                isFillingTheJug = true;
            }
        }

        float length = Mathf.Min(currentLength, targetLength);

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 pos = start + Vector3.down * length * t;
            pos.y -= gravityCurve * t * t;
            line.SetPosition(i, pos);
        }

        if (splashParticles != null && length >= targetLength)
        {
            splashParticles.transform.position = end;
            if (!splashParticles.isPlaying)
                splashParticles.Play();
        }
    }
}
