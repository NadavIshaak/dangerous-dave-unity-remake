using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int _value = 100;
    [SerializeField] private AudioClip _collectSound;
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        SoundManager.Instance.PlaySound(_collectSound, transform, 1);
        ScoreManager.Instance.AddScore(_value);
        Destroy(gameObject);
    }
}