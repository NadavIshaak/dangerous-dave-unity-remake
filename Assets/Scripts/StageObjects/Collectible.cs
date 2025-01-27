using UnityEngine;
using UnityEngine.Serialization;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int _value = 100;

    [FormerlySerializedAs("_collectSound")] [SerializeField]
    private AudioClip collectSound;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        SoundManager.Instance.PlaySound(collectSound, transform, 1, false, false, false);
        CurrentLevelManagar.instance.AddScore(_value);
        Destroy(gameObject);
    }
}