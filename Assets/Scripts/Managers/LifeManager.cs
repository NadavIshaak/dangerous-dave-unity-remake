using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoSingleton<LifeManager>
{
    private InputSystem_Actions _controls;
    private bool _isDead;
    private int _lives = 4;
    private void Update()
    {
        if (!_isDead) return;
        if (!_controls.Player.Jump.triggered) return;
        _controls.Disable();
        Application.Quit();
    }

    public void RemoveLife()
    {
        _lives--;
        if (_lives == 0)
        {
            _isDead = true;
            _controls = new InputSystem_Actions();
            return;
        }

        UpdateLifeDisplay();
    }
    public int GetLife()
    {
        return _lives;
    }

    private void UpdateLifeDisplay()
    {
        UIManager.Instance.UpdateLife(_lives);
    }
}