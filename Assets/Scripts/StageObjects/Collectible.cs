using UnityEngine;
using UnityEngine.Serialization;

/**
 * This class is responsible for managing the collectible objects.
 * It handles the collection of the collectible objects by the player.
 */
public class Collectible : MonoBehaviour
{
    [SerializeField] private int _value = 100;

    [FormerlySerializedAs("_collectSound")] [SerializeField]
    private AudioClip collectSound;

    /**
     * When the player collides with the collectible object, the collectible object is destroyed,
     * and the player's score is increased.
     */
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        SoundManager.Instance.PlaySound(collectSound, transform, 1, false, false, false);
        CurrentLevelManagar.instance.AddScore(_value);
        Destroy(gameObject);
    }
}