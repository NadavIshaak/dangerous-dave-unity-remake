using UnityEngine;

/**
 * This class is responsible for handling the trophy object.
 */
public class Trophy : Collectible
{
    /**
     * When the player collides with the trophy, the trophy is collected.
     */
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        CurrentLevelManagar.instance.ThrophyCollected();
    }
}