using UnityEngine;

public class GunScript : Collectible
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        CurrentLevelManagar.instance.SetHasGun(true);
    }
}