using System.Collections;
using UnityEngine;

/**
 * This class is responsible for handling the enemy bullet pool.
 */
public class EnemyBulletPool : MonoPool<EnemyBullet>
{
    public override void Return(EnemyBullet enemyBullet)
    {
        StartCoroutine(WaitThenReturn(enemyBullet, 1f));
    }


    private IEnumerator WaitThenReturn(EnemyBullet enemyBullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        base.Return(enemyBullet); // Now run the actual return logic
    }
}