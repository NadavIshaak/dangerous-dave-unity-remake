using UnityEngine;

/**
 * This class is responsible for handling the enemy's shooting.
 */
public class EnemyShooting : MonoBehaviour
{
    [SerializeField] private float shootInterval = 2f; // Time interval between shots
    [SerializeField] private float projectileSpeed = 3f; // Speed of the projectile
    [SerializeField] private bool isRandom = false; // If true, the enemy will shoot at random intervals
    [SerializeField] private float minRandomInterval = 1f; // Minimum random interval
    private PlayerMovement _player;
    private Transform _shootPoint; // The point from which the projectile will be shot
    private float _shootTimer;
    /**
     * if the shooting is random set to a set random variable
     * if not set cooldown to the interval
     */
    private void Start()
    {
        _shootTimer = !isRandom ? shootInterval : Random.Range(minRandomInterval, shootInterval);

        CurrentLevelManagar.instance.PlayerManager.OnInstantiatedPlayer += SetNewPlayer;
    }

    /**
     * if the cooldown is lower the 0 make enemy shoot
     */
    private void Update()
    {
        _shootTimer -= Time.deltaTime;

        if (_shootTimer <= 0f)
        {
            Shoot();
            _shootTimer = !isRandom ? shootInterval : Random.Range(minRandomInterval, shootInterval);
        }
    }

    private void OnDisable()
    {
        CurrentLevelManagar.instance.PlayerManager.OnInstantiatedPlayer -= SetNewPlayer;
    }

    /**
     * When a new player is instanced set the enemy to track its
     * location and shoot at it
     */
    private void SetNewPlayer()
    {
        _player = FindFirstObjectByType<PlayerMovement>();
    }

    /**
     * Shoot a bullet at the player
     */
    private void Shoot()
    {
        if (!_player) return;
        var direction = _player.transform.position.x > transform.position.x ? Vector3.right : Vector3.left;
        _shootPoint = transform;
        ShootBullet(direction);
    }

    private void ShootBullet(Vector3 direction)
    {
        var enemyBullet = EnemyBulletPool.Instance.Get();
        enemyBullet.transform.position = _shootPoint.position;

        // Flip the bullet sprite based on the direction
        var spriteRenderer = enemyBullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer is not null) spriteRenderer.flipX = direction == Vector3.right;
        var rb = enemyBullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;
    }
}