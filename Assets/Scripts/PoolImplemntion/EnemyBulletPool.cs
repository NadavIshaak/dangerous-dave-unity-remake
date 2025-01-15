using UnityEngine;
using System.Collections;
public class EnemyBulletPool : MonoPool<Bullet>
{


    public override void Return(Bullet bullet)
    {
         StartCoroutine(WaitThenReturn(bullet, 1f));
    }
    

    private IEnumerator WaitThenReturn(Bullet bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        base.Return(bullet); // Now run the actual return logic
    }
}
