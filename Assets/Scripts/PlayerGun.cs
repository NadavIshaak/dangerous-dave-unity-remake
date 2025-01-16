using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    private Transform _shootPoint; // The point from which the projectile will be shot
    [SerializeField] private float shootInterval = 2f; // Time interval between shots
    [SerializeField] private float projectileSpeed = 3f; // Speed of the projectile
    private PlayerMovement _player;
    private float _shootTimer;

    private void Start()
    {
        _shootTimer = shootInterval;
        _player = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        _shootTimer -= Time.deltaTime;

        if (_shootTimer <= 0f)
        {
            CheckForShoot();
        }
    }

    private void CheckForShoot()
    {
        if (_player is null || !_player.GetCanShoot()||!_player.GetControls().Player.Attack.triggered) return;

        var direction = (_player.currentState. > 0) ? Vector3.right : Vector3.left;
        _shootPoint=transform;
        var bullet = PlayerBulletPool.Instance.Get();
        bullet.transform.position = _shootPoint.position;

        // Flip the bullet sprite based on the direction
        var spriteRenderer = bullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer is not null)
        {
            spriteRenderer.flipX = (direction == Vector3.left);
        }

        var rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;
        _shootTimer = shootInterval;
    }
}