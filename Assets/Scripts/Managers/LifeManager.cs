using UnityEngine;

/**
 * This class is responsible for handling the player's death screen.
 */
public class LifeManager : MonoBehaviour
{
    private InputSystem_Actions _controls;
    private bool _isDead;


    private void Start()
    {
        CurrentLevelManagar.instance.PlayerManager.OnLifeChange += RemoveLife;
    }

    /**
     * check if the player is dead and for input so we can close the game
     */
    private void Update()
    {
        if (!_isDead) return;
        if (!_controls.Player.Jump.triggered) return;
        _controls.Disable();
        Application.Quit();
    }

    private void OnDisable()
    {
        CurrentLevelManagar.instance.PlayerManager.OnLifeChange -= RemoveLife;
    }

    /**
     * remove a life from the player, if the player has no more lives, set the player to dead
     */
    private void RemoveLife(int life)
    {
        if (life != 0) return;
        _isDead = true;
        _controls = new InputSystem_Actions();
        _controls.Enable();
    }
}