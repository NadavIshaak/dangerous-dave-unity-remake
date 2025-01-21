using UnityEngine;

public class SoundObject : MonoBehaviour, IPoolable
{
    //class to handle the sound objects
    [SerializeField] private AudioSource audioSource; // The audio source component

    private void Update()
    {
        // Check for cheat code
        CheckForCheatCode();
    }

    public void Reset()
    {
        // Reset the audio source component
        audioSource.clip = null;
        audioSource.volume = 1f;
        audioSource.pitch = 1f;
        audioSource.spatialBlend = 0f;
        audioSource.loop = false;
    }

    private void CheckForCheatCode()
    {
        //send back the sound object to the pool
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6))
            SoundPool.Instance.ImmediateReturn(this);
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
    }
}