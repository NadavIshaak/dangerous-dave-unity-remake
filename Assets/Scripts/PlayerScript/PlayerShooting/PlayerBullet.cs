using UnityEngine;

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