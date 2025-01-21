using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //class to handle the sounds in the game
    public static SoundManager Instance;

    private SoundObject _currentSoundObject;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the SoundManager persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopSound()
    {
        if (_currentSoundObject is null) return;
        SoundPool.Instance.ImmediateReturn(_currentSoundObject);
        _currentSoundObject = null;
    }


    public void PlaySound(AudioClip clip, Transform spawnPosition, float volume, bool loop = false,
        bool shouldKeep = false, bool shouldStop = true)
    {
        if (shouldStop) StopSound();
        var soundObject = SoundPool.Instance.Get();
        //Create an instance of the audio source
        var audioSource = soundObject.GetComponent<AudioSource>();
        //Set the position of the audio source
        audioSource.transform.position = spawnPosition.position;
        //set the clip of the audio source
        audioSource.clip = clip;
        //set the volume of the audio source
        audioSource.volume = volume;
        audioSource.loop = loop;
        //Play the audio clip
        audioSource.Play();
        if (shouldKeep) _currentSoundObject = soundObject;
        //return the audio source after the clip has finished playing
        SoundPool.Instance.Return(soundObject);
    }
}