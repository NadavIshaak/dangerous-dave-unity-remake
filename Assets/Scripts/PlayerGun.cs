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
         PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
        if(!player.GetCanShoot())
        return;
        }
        else
        {
            return;
        }

        Vector3 direction = (player.position.x > transform.position.x) ? Vector3.right : Vector3.left;
        shootPoint = transform;
        Bullet bullet = EnemyBulletPool.Instance.Get();
        bullet.transform.position = shootPoint.position;

         // Flip the bullet sprite based on the direction
        SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direction == Vector3.right);
        }
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;
    }
}