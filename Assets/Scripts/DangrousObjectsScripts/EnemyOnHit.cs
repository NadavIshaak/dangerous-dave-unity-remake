using UnityEngine;
using UnityEngine.Serialization;

public class EnemyOnHit : MonoBehaviour
{
    private static readonly int Die = Animator.StringToHash("Die");
    [SerializeField] private float timeToDestroy = 3f;
    private BoxCollider2D _boxCollider;
    private Animator _animator;
    [SerializeField] private AudioClip deathSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _animator=GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("PlayerBullet")) return;
        _animator.SetTrigger(Die);
        _boxCollider.enabled = false;
        SoundManager.Instance.PlaySound(deathSound, transform, 1);
        // Destroy the enemy
        Destroy(gameObject, timeToDestroy);
    }
}
