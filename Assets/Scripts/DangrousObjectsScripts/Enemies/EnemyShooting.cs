using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] private float shootInterval = 2f; // Time interval between shots
    [SerializeField] private float projectileSpeed = 3f; // Speed of the projectile
    private PlayerMovement _player;
    private Transform _shootPoint; // The point from which the projectile will be shot
    private float _shootTimer;

    private void Start()
    {
        _shootTimer = shootInterval;
        CurrentLevelManagar.instance.PlayerManager.OnInstantiatedPlayer += SetNewPlayer;
    }

    private void Update()
    {
        _shootTimer -= Time.deltaTime;

        if (_shootTimer <= 0f)
        {
            Shoot();
            _shootTimer = shootInterval;
        }
    }

    private void OnDisable()
    {
        CurrentLevelManagar.instance.PlayerManager.OnInstantiatedPlayer -= SetNewPlayer;
    }

    private void SetNewPlayer()
    {
        _player = FindFirstObjectByType<PlayerMovement>();
    }

    private void Shoot()
    {
        if (!_player) return;
        var direction = _player.transform.position.x > transform.position.x ? Vector3.right : Vector3.left;
        _shootPoint = transform;
        var enemyBullet = EnemyBulletPool.Instance.Get();
        enemyBullet.transform.position = _shootPoint.position;

        // Flip the bullet sprite based on the direction
        var spriteRenderer = enemyBullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer is not null) spriteRenderer.flipX = direction == Vector3.right;
        var rb = enemyBullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;
    }
}