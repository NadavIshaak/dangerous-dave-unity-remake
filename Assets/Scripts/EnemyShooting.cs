using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    private Transform shootPoint; // The point from which the projectile will be shot
    [SerializeField] float shootInterval = 2f; // Time interval between shots
    [SerializeField] float projectileSpeed = 3f; // Speed of the projectile
    PlayerMovement player=null;
      private float shootTimer;

    void Start()
    {
        shootTimer = shootInterval;
        GameManager.Instance.OnInstantiatedPlayer+=SetNewPlayer;
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }
     private void OnDisable() {
        GameManager.Instance.OnInstantiatedPlayer-=SetNewPlayer;
    }

    public void SetNewPlayer()
    {
        player = Object.FindFirstObjectByType<PlayerMovement>();
    }

    void Shoot()
    {
        if (player == null) return;
        Vector3 direction = (player.transform.position.x > transform.position.x) ? Vector3.right : Vector3.left;
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
