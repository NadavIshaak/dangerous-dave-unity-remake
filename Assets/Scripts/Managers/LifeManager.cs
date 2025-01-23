using System;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    private InputSystem_Actions _controls;
    private bool _isDead;

    private void OnEnable()
    {
        CurrentLevelManagar.Instance.OnLifeChange += RemoveLife;
    }

    private void Update()
    {
        if (!_isDead) return;
        if (!_controls.Player.Jump.triggered) return;
        _controls.Disable();
        Application.Quit();
    }

    private void RemoveLife(int life)
    {
        if (life != 0) return;
        _isDead = true;
        _controls = new InputSystem_Actions();
    }
}