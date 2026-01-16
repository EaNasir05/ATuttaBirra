using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] private AudioSource[] sources;
    private bool[] availableSources;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        availableSources = new bool[sources.Length];
        for (int i = 0; i < availableSources.Length; i++)
        {
            availableSources[i] = true;
        }
    }

    private int GetTheFirstAvailableSource()
    {
        for (int i = 0; i < availableSources.Length; i++)
        {
            if (availableSources[i])
                return i;
        }
        return -1;
    }

    public void PlayClip(AudioClip clip, float volume)
    {
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return;
        availableSources[i] = false;
        sources[i].clip = clip;
        sources[i].volume = volume;
        sources[i].loop = false;
        sources[i].Play();
        StartCoroutine(MakeSourceAvailable(clip.length, i));
    }

    public void PlayClipWithRandomPitch(AudioClip clip, float volume)
    {
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return;
        availableSources[i] = false;
        sources[i].clip = clip;
        sources[i].volume = volume;
        sources[i].loop = false;
        sources[i].pitch = Random.Range(0.85f, 1.15f);
        sources[i].Play();
        StartCoroutine(MakeSourceAvailable(clip.length, i));
    }

    public int PlayClipWithRandomPitchAndReturnIndex(AudioClip clip, float volume)
    {
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return -1;
        availableSources[i] = false;
        sources[i].clip = clip;
        sources[i].volume = volume;
        sources[i].loop = false;
        sources[i].pitch = Random.Range(0.85f, 1.15f);
        sources[i].Play();
        StartCoroutine(MakeSourceAvailable(clip.length, i));
        return i;
    }

    public int PlayClipInLoop(AudioClip clip, float volume)
    {
        int i = GetTheFirstAvailableSource();
        if (i == -1)
            return -1;
        availableSources[i] = false;
        sources[i].clip = clip;
        sources[i].volume = volume;
        sources[i].loop = true;
        sources[i].pitch = Random.Range(0.9f, 1.1f);
        sources[i].Play();
        return i;
    }

    private IEnumerator MakeSourceAvailable(float delay, int index)
    {
        yield return new WaitForSeconds(delay);
        availableSources[index] = true;
    }

    public void StopClipInLoop(int index)
    {
        sources[index].Stop();
        availableSources[index] = true;
    }

    public void StopClip(int index)
    {
        sources[index].Stop();
    }
}
