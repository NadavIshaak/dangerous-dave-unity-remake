using System.Collections;
using UnityEngine;

/**
 * This class is responsible for handling the player bullet pool.
 */
public class PlayerBulletPool : MonoPool<PlayerBullet>
{
    public override void Return(PlayerBullet bullet)
    {
        StartCoroutine(WaitThenReturn(bullet, 1f));
    }


    private IEnumerator WaitThenReturn(PlayerBullet bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        base.Return(bullet); // Now run the actual return logic
    }
}