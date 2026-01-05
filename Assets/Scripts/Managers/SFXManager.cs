using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] private AudioSource soundEffectObject;
    [SerializeField] private SoundEffectsClips sfxClips;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySFXClip(AudioClip clip, float volume)
    {
        AudioSource newSource = Instantiate(soundEffectObject, Vector3.zero, Quaternion.identity);
        newSource.clip = clip;
        newSource.volume = volume;
        newSource.Play();
        StartCoroutine(DestroyAfterPlay(newSource));
    }

    public void PlaySFXClip(int index)
    {
        AudioSource newSource = Instantiate(soundEffectObject, Vector3.zero, Quaternion.identity);
        newSource.clip = sfxClips.soundEffectsList[index].clip;
        newSource.volume = sfxClips.soundEffectsList[index].volume;
        newSource.Play();
        StartCoroutine(DestroyAfterPlay(newSource));
    }

    public void PlaySFXClip(string name)
    {
        AudioSource newSource = Instantiate(soundEffectObject, Vector3.zero, Quaternion.identity);
        SoundEffectClip sfx = sfxClips.GetSoundEffectClip(name);
        newSource.clip = sfx.clip;
        newSource.volume = sfx.volume;
        newSource.Play();
        StartCoroutine(DestroyAfterPlay(newSource));
    }

    private IEnumerator DestroyAfterPlay(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        Destroy(source.gameObject);
    }
}
