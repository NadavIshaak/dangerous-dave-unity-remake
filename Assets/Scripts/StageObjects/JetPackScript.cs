using UnityEngine;

/**
 * This class is responsible for handling the jetpack collectible.
 */
public class JetPackScript : Collectible
{
    /**
     * Set the player's jetpack to true when the player collides with the jetpack.
     */
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        CurrentLevelManagar.instance.SetHasJetPack(true);
    }
}