using UnityEngine;

public class JetPackScript : Collectible
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        var player = FindFirstObjectByType<PlayerMovement>();
        if (player == null) return;
        CurrentLevelManagar.instance.SetHasJetPack(true);
        player.SetHasJetPack(true);
    }
}