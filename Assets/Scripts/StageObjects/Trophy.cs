using UnityEngine;

public class Trophy : Collectible
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        CurrentLevelManagar.Instance.ThrophyCollected();
    }
}