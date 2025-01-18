using UnityEngine;
using UnityEngine.Audio;
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

    public void stopSound()
    {
        if (_currentSoundObject == null)
        {
            return;
        }
        SoundPool.Instance.ImmediateReturn(_currentSoundObject);
    }
   
   
   

    public void PlaySound(AudioClip clip, Transform spawnPosition, float volume,bool loop=false,bool shouldKeep=false,bool shouldStop=true){
        if(shouldStop)
        {
            stopSound();
        }
        SoundObject soundObject = SoundPool.Instance.Get();
        //Create an instance of the audio source
        AudioSource audioSource= soundObject.GetComponent<AudioSource>();
        //Set the position of the audio source
        audioSource.transform.position = spawnPosition.position;
        //set the clip of the audio source
        audioSource.clip = clip;
        //set the volume of the audio source
        audioSource.volume =volume;
        //Play the audio clip
        audioSource.Play();
        if(loop){
            audioSource.loop=true;
        }
        else{
            audioSource.loop=false;
        }
        if(shouldKeep){
            _currentSoundObject=soundObject;
        }
        //return the audio source after the clip has finished playing
        SoundPool.Instance.Return(soundObject);
    }
}