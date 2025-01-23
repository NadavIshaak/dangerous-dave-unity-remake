using UnityEngine;

public class SoundObject : MonoBehaviour, IPoolable
{
    //class to handle the sound objects
    [SerializeField] private AudioSource audioSource; // The audio source component
    public void Reset()
    {
        // Reset the audio source component
        audioSource.clip = null;
        audioSource.volume = 1f;
        audioSource.pitch = 1f;
        audioSource.spatialBlend = 0f;
        audioSource.loop = false;
    }
    public AudioSource GetAudioSource()
    {
        return audioSource;
    }
}