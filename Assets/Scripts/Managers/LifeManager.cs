using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoSingleton<LifeManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Image _oneLifeRenderer;
    [SerializeField] private Image _twoLifeRenderer;
    [SerializeField] private Image _threeLifeRenderer;
    [SerializeField] private Image _DeadRenderer;
    private InputSystem_Actions _controls;
    private bool _isDead;
    private int _lives = 4;

    private void Update()
    {
        if (!_isDead) return;
        if (_controls.Player.Jump.triggered) Application.Quit();
    }

    public void RemoveLife()
    {
        _lives--;
        if (_lives == 0)
        {
            _isDead = true;
            _DeadRenderer.enabled = true;
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
        switch (_lives)
        {
            case 3:
                _threeLifeRenderer.enabled = false; break;
            case 2:
                _twoLifeRenderer.enabled = false; break;
            case 1:
                _oneLifeRenderer.enabled = false; break;
        }
    }
}