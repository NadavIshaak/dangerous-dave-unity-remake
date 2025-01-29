using UnityEngine;

/**
 * This class is responsible for handling the dangerous objects in the game.
 */
public class DangerousObject : MonoBehaviour
{
    [SerializeField] private AudioClip deathSound;

    /**
     * check if the player collides with the dangerous object
     * if the player collides with the dangerous object, kill the player
     */
    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) KillPlayer();
    }

    /**
     * kill the player,triggers the player death event and plays the death sound
     */
    private void KillPlayer()
    {
        CurrentLevelManagar.instance.TriggerPlayerDeath();
        SoundManager.Instance.PlaySound(deathSound, transform, 1);
    }
}