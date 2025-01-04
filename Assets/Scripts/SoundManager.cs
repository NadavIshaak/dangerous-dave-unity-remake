using UnityEngine;
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour
{
    //class to handle the sounds in the game
public static SoundManager Instance;

[SerializeField] private AudioClip backgroundMusicClip;// The background music clip
private AudioSource backgroundMusicSource;//instance of the audio source for the background music
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

    private void Start(){
        //Start the background music
        startBackgroundMusic();
    }
    private void startBackgroundMusic(){
        //Create an instance of the audio source
        //and play the background music with it
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource.clip = backgroundMusicClip;
        backgroundMusicSource.loop = true;
        backgroundMusicSource.spatialBlend = 0;
        backgroundMusicSource.Play();
    }
   

    public void PlaySound(AudioClip clip, Transform spawnPosition, float volume){
        SoundObject soundObject = SoundPool.Instance.Get();
        //Create an instance of the audio source
        AudioSource audioSource= soundObject.GetComponent<AudioSource>();
        //Set the position of the audio source
        audioSource.transform.position = spawnPosition.position;
        //set the clip of the audio source
        audioSource.clip = clip;
        //set the volume of the audio source
        audioSource.volume =volume;
        //set random pitch of the audio source
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        //set loop to false
        audioSource.loop = false;
        //set the spatial blend of the audio source
        audioSource.spatialBlend = 1;
        //Play the audio clip
        audioSource.Play();
        //return the audio source after the clip has finished playing
        SoundPool.Instance.Return(soundObject);
    }
}