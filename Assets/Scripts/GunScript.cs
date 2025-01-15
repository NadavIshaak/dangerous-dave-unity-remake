using UnityEngine;

public class GunScript :Collectible
{
    protected override void OnTriggerEnter2D(Collider2D other)
   {
        base.OnTriggerEnter2D(other);
       PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            player.SetCanShoot(true);
        }
   }
}
