using UnityEngine;
using System.Collections;
public class SoundPool : MonoPool<SoundObject>
{


    public override void Return(SoundObject sound)
    {
        if(sound.GetAudioSource().loop.Equals(true))
        {
            return;
        }
        //return the sound to the pool after the sound has finished playing
         StartCoroutine(WaitThenReturn(sound, sound.GetAudioSource().clip.length));
    }
    

    private IEnumerator WaitThenReturn(SoundObject sound, float delay)
    {
        yield return new WaitForSeconds(delay);
        base.Return(sound); // Now run the actual return logic
    }
}
