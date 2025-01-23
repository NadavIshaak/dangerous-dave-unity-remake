using UnityEngine;

public class EnemyBullet : DangerousObject, IPoolable
{
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