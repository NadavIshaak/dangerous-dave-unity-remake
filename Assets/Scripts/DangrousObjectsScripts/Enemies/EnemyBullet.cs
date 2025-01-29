using UnityEngine;

/**
 * This class is responsible for handling the enemy's bullets.
 * It is used to return the bullet to the pool when it collides with an object.
 */
public class EnemyBullet : DangerousObject, IPoolable
{
    /**
     * When the bullet collides with an object, check if the object is an enemy.
     * If the object is an enemy, return the bullet to the pool.
     */
    public override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        if (other.gameObject.CompareTag("Enemy")) return;
        EnemyBulletPool.Instance.ImmediateReturn(this);
    }

    // ReSharper disable once Unity.RedundantEventFunction
    public void Reset()
    {
        // Reset the object to its default state
    }
}