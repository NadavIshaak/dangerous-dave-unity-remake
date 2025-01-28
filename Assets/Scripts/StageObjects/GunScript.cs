using UnityEngine;

    /**
     * This class is responsible for handling the gun collectible.
     */
public class GunScript : Collectible
{
    /**
     * Set the player to have a gun when the player collides with the gun collectible.
     */
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        CurrentLevelManagar.instance.SetHasGun(true);
    }
}