using UnityEngine;

public class AudioInstantiator : MonoBehaviour
{
    public void InstantiateAudio(GameObject audioObject)
    {
        AudioSource audio =  Instantiate(audioObject,null).GetComponent<AudioSource>();
        if(audio == null)
        {
            Debug.Log("AudioObject parametter doesn't have an AudioSource Component");
            Destroy(audio.gameObject);
            return;
        }
        
        audio.Play();
        Destroy(audio.gameObject,audio.clip.length);
    }
}
