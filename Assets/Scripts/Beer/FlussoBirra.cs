using UnityEngine;

public class LiquidStreamToggle : MonoBehaviour
{
    [Header("Flow Settings")]
    public Transform startPoint;
    public float maxDistance = 5f;

    [Header("Collision and DrinkSystem")]
    public LayerMask collisionMask;
    public DrinkSystem drinkSystem;
    public float beerGainRate = 5f;

    [Header("Particles")]
    public ParticleSystem streamParticles;
    public ParticleSystem splashParticles;

    [Header("Enough Beer")]
    public GameObject EnoughBeer;

    private bool isFlowing = false;
    private bool wasFillingTheJug = false;
    private bool isFillingTheJug = false;

    private AudioSource audioSource;
    private AudioSource loopAudioSource;
    private float startingLoopSeconds;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        loopAudioSource = startPoint.GetComponent<AudioSource>();
        startingLoopSeconds = loopAudioSource.clip.length * 0.25f;

        if (streamParticles != null)
            streamParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (splashParticles != null)
            splashParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (EnoughBeer != null)
            EnoughBeer.SetActive(false);
    }

    void Update()
    {
        if (!isFlowing)
        {
            isFillingTheJug = false;
            CheckFillingTheJug();
            CheckEnoughBeer(); 
            return;
        }

        RaycastCheck();
        CheckFillingTheJug();
        CheckEnoughBeer();

        if (loopAudioSource.isPlaying && loopAudioSource.time >= loopAudioSource.clip.length)
            loopAudioSource.time = startingLoopSeconds;
    }

    

    public void SetFlow(bool active)
    {
        if (isFlowing == active)
            return;

        isFlowing = active;

        if (active)
        {
            if (streamParticles != null)
                streamParticles.Play();

            StartLoopClip();
        }
        else
        {
            if (streamParticles != null)
                streamParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            if (splashParticles != null)
                splashParticles.Stop();

            if (EnoughBeer != null)                 
                EnoughBeer.SetActive(false);

            StopLoopClip();
        }
    }

    
    void RaycastCheck()
    {
        Vector3 start = startPoint.position;
        Vector3 dir = Vector3.down;
        isFillingTheJug = false;

        if (Physics.Raycast(start, dir, out RaycastHit hit, maxDistance, collisionMask))
        {
            if (splashParticles != null)
            {
                splashParticles.transform.position = hit.point;

                if (!splashParticles.isPlaying)
                    splashParticles.Play();
            }

            if (hit.collider.CompareTag("StreamTarget"))
            {
                drinkSystem.GainBeer(beerGainRate * Time.deltaTime);
                isFillingTheJug = true;
            }
        }
        else
        {
            if (splashParticles != null && splashParticles.isPlaying)
                splashParticles.Stop();
        }
    }

   

    private void CheckEnoughBeer()
    {
        if (EnoughBeer == null || drinkSystem == null)
            return;

        bool overLimit = drinkSystem.GetBeerFill() <= drinkSystem.GetMinFill();

        if (isFillingTheJug && overLimit)
            EnoughBeer.SetActive(true);
        else
            EnoughBeer.SetActive(false);
    }

    private void CheckFillingTheJug()
    {
        if (!wasFillingTheJug && isFillingTheJug)
        {
            drinkSystem.receivingBeer = true;
            wasFillingTheJug = true;

            float perc = 1 - ((drinkSystem.GetBeerFill() - drinkSystem.GetMinFill()) / (drinkSystem.GetMaxFill() - drinkSystem.GetMinFill()));

            audioSource.Play();
            audioSource.time = Mathf.Clamp(perc * audioSource.clip.length, 0, audioSource.clip.length);
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

    public bool IsFlowing() => isFlowing;
}

