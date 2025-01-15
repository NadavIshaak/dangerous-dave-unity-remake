using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    private Transform shootPoint; // The point from which the projectile will be shot
    [SerializeField] float shootInterval = 2f; // Time interval between shots
    [SerializeField] float projectileSpeed = 3f; // Speed of the projectile
    private float shootTimer;

    void Start()
    {
        shootTimer = shootInterval;
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            checkForShoot();
        }
    }

    void checkForShoot()
    {
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player == null || !player.GetCanShoot()||!player.GetControls().Player.Attack.triggered) return;

        Vector3 direction = (player.transform.localScale.x > 0) ? Vector3.right : Vector3.left;
        shootPoint=transform;
        PlayerBullet bullet = PlayerBulletPool.Instance.Get();
        bullet.transform.position = shootPoint.position;

        // Flip the bullet sprite based on the direction
        SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direction == Vector3.left);
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;
    }
}