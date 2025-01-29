using UnityEngine;

/**
 * This class is responsible for handling the player's bullets.
 * It is used to return the bullet to the pool when it collides with an object.
 */
public class PlayerBullet : MonoBehaviour, IPoolable
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerBulletPool.Instance.ImmediateReturn(this);
    }

    public void Reset()
    {
    }
}