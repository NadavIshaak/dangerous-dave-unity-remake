using UnityEngine;

public class PlayerBullet : MonoBehaviour, IPoolable
{

    protected virtual void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
        PlayerBulletPool.Instance.ImmediateReturn(this);
    }
    public void Reset()
    {
      
    }
}