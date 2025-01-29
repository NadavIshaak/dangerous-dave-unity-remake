using UnityEngine;

/**
 * This class is responsible for handling the player's death screen.
 */
public class LifeManager : MonoBehaviour
{
    private InputSystem_Actions _controls;


    private void Start()
    {
        CurrentLevelManagar.instance.PlayerManager.OnLifeChange += RemoveLife;
        _controls = new InputSystem_Actions();
        _controls.Player.Quit.performed += context => Application.Quit();
        _controls.Player.Jump.performed += context => Application.Quit();
        _controls.Disable();
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
        _controls.Enable();
    }
}