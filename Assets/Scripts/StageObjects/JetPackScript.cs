using UnityEngine;

public class JetPackScript : Collectible
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        CurrentLevelManagar.instance.SetHasJetPack(true);
    }
}