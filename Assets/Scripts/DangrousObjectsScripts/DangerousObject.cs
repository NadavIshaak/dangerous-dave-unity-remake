using UnityEngine;

public class DangerousObject : MonoBehaviour
{
    [SerializeField] private AudioClip deathSound;

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) KillPlayer();
    }

    private void KillPlayer()
    {
        CurrentLevelManagar.instance.TriggerPlayerDeath();
        SoundManager.Instance.PlaySound(deathSound, transform, 1);
    }
}