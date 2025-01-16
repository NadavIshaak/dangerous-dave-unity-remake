using UnityEngine;

public class Bullet : DangerousObject,IPoolable
{
    public override void OnCollisionEnter2D(Collision2D other) {
        base.OnCollisionEnter2D(other);
        if (other.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        EnemyBulletPool.Instance.ImmediateReturn(this);
    }
    public void Reset()
    {
        // Reset the object to its default state
    }
}