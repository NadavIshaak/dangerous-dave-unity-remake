using UnityEngine;

public class JetPackScript :Collectible
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        var player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player == null) return;
        GameManager.instance.SetHasJetPack(true);
        player.SetHasJetPack(true);
    }
}
