using UnityEngine;

public class LifeManager : MonoBehaviour
{
    private InputSystem_Actions _controls;
    private bool _isDead;


    private void Start()
    {
        CurrentLevelManagar.Instance.PlayerManager.OnLifeChange += RemoveLife;
    }

    private void Update()
    {
        if (!_isDead) return;
        if (!_controls.Player.Jump.triggered) return;
        _controls.Disable();
        Application.Quit();
    }

    private void OnDisable()
    {
        CurrentLevelManagar.Instance.PlayerManager.OnLifeChange -= RemoveLife;
    }

    private void RemoveLife(int life)
    {
        if (life != 0) return;
        _isDead = true;
        _controls = new InputSystem_Actions();
        _controls.Enable();
    }
}