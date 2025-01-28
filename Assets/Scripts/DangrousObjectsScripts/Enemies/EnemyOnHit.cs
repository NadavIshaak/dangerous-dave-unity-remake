using UnityEngine;

/**
 * This class is responsible for handling the enemy's death.
 */
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

    /**
     * When the enemy is hit by a bullet, it dies.
     */
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("PlayerBullet")) return;
        OnDeath();
    }

    /**
     * When the enemy is hit by a
     * bullet, it dies.
     * When the enemy dies, it plays the death animation, disables the enemy's shooting and movement,
     * and destroys the enemy after a set time.
     */
    private void OnDeath()
    {
        var enemyShooting = GetComponent<EnemyShooting>();
        if (enemyShooting is not null) enemyShooting.enabled = false;
        var enemySpinnerMovement = GetComponent<EnemySpinnerMovement>();
        if (enemySpinnerMovement is not null) enemySpinnerMovement.enabled = false;
        _animator.SetTrigger(Die);
        _boxCollider.enabled = false;
        SoundManager.Instance.PlaySound(deathSound, transform, 1);
        // Destroy the enemy
        Destroy(gameObject, timeToDestroy);
    }
}