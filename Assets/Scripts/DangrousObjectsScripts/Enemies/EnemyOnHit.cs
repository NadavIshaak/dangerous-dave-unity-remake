using UnityEngine;

public class EnemyOnHit : MonoBehaviour
{
    private static readonly int Die = Animator.StringToHash("Die");
    [SerializeField] private float timeToDestroy = 3f;
    [SerializeField] private AudioClip deathSound;
    private Animator _animator;
    private BoxCollider2D _boxCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _animator = GetComponent<Animator>();
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