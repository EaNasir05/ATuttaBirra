using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffectsClips", menuName = "ScriptableObjects/SoundEffectsClips")]
public class SoundEffectsClips : ScriptableObject
{
    public List<SoundEffectClip> soundEffectsList;

    public SoundEffectClip GetSoundEffectClip(string name)
    {
        foreach (SoundEffectClip clip in soundEffectsList)
        {
            if (clip.name.Equals(name))
            {
                return clip;
            }
        }
        return null;
    }
}

[Serializable]
public class SoundEffectClip
{
    public string name;
    public AudioClip clip;
    public float volume;
}