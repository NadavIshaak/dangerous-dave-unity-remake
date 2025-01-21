using UnityEngine;

public class GunScript : Collectible
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        var player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            player.SetCanShoot(true);
            CurrentLevelManagar.instance.SetHasGun(true);
        }
    }
}