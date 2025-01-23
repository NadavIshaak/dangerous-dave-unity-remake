using System;
using Triggers;
using Unity.Cinemachine;
using UnityEngine;

public class CurrentLevelManagar : MonoSingleton<CurrentLevelManagar>
{
    public static CurrentLevelManagar instance;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject[] _stagesSpawns;
    [SerializeField] private SpriteRenderer _StageWinWalkRenderer;
    [SerializeField] private Sprite[] _StageWinWalkSprite;
    [SerializeField] private CinemachineSplineCart[] dollyCart;
    private int _currentLevel = 1;
    private bool _hasGun;
    private bool _hasJetPack;
    private float _currentJetPackFuel;
    private bool _trophyCollected;
    private int _currentLife = 4;

 

    public event Action OnVictoryWalkStart;
    public event Action OnInstantiatedPlayer;
    public event Action OnGameOver;
    public event Action<int> OnLevelChange;
    public event Action<bool> OnGunChange;
    public event Action<bool> OnJetPackChange;
    public event Action<bool> OnTrophyChange; 
    public event Action<float,float> OnFuelChange;
    public event Action<string,bool> OnShowTriggerText;
    public event Action<int> OnLifeChange; 
    public static event Action<string, TriggerRequirements> OnShowTriggerTextWithReq;
    public static event Action OnHideTriggerText;
    
    private void OnEnable()
    {
        // Subscribe to both events
        OnShowTriggerTextWithReq += ShowTextWithRequirement;
        OnHideTriggerText        += HideText;
    }

    private void OnDisable()
    {
        OnShowTriggerTextWithReq -= ShowTextWithRequirement;
        OnHideTriggerText        -= HideText;
    }
    private void ShowTextWithRequirement(string message, TriggerRequirements requirement)
    {
        if (requirement.HasGun==_hasGun && 
            requirement.HasJetPack==_hasJetPack &&
            requirement.RequiredScore<ScoreManager.Instance.GetScore() && 
            requirement.RequiredLevel==_currentLevel
            && requirement.HasTrophy==_trophyCollected)
        {
            OnShowTriggerText?.Invoke(message,true);
        }
    }
    public static void HideTriggerText()
    {
        OnHideTriggerText?.Invoke();
    }
    public static void ShowTriggerText(string message,TriggerRequirements requirement)
    {
        OnShowTriggerTextWithReq?.Invoke(message,requirement);
    }

    /// <summary>
    /// Called by OnHideTriggerText. We simply clear or hide the text.
    /// </summary>
    private  void HideText()
    {
        OnShowTriggerText?.Invoke("",false);
    }
    public void ThrophyCollected()
    {
        OnTrophyChange?.Invoke(true);
        _trophyCollected = true;
    }

    public void UpdateFuelBar(float currentFuel, float maxFuel)
    {
        OnFuelChange?.Invoke(currentFuel,maxFuel);
    }

    public void DoorReached()
    {
        if (!_trophyCollected) return;
        _trophyCollected = false;
        _StageWinWalkRenderer.sprite = _StageWinWalkSprite[_currentLevel - 1];
        _hasGun = false;
        _hasJetPack = false;
        OnTrophyChange?.Invoke(false);
        OnVictoryWalkStart?.Invoke();
    }

    public void OnVictoryWalkEnd()
    {
        // Handle the end of the victory walk
        // Example: Spawn the player in the next area
        SetHasGun(false);
        SetHasJetPack(false);
        _currentLevel++;
        if (_currentLevel == 4)
        {
            OnGameOver?.Invoke();
            return;
        }
        OnLevelChange?.Invoke(_currentLevel);
        InstantiatePlayer();
    }

    public void InstantiatePlayer()
    {
        if (_currentLife==0)
        {
            return;
        }
        var spawnPosition = _stagesSpawns[_currentLevel].transform.position;
        if (_currentLevel is 2 or 3) dollyCart[_currentLevel - 2].SplinePosition = 0;
        Instantiate(Player, spawnPosition, Quaternion.identity);
        OnInstantiatedPlayer?.Invoke();
    }
    

    public void TriggerPlayerDeath()
    {
        _currentLife--;
        OnLifeChange?.Invoke(_currentLife);
        var player = FindFirstObjectByType<PlayerMovement>();
        if (player is null) return;
        player.TriggerDeath();
        Invoke(nameof(InstantiatePlayer), 3.1f);
    }

    public void SetHasGun(bool hasGun)
    {
        _hasGun = hasGun;
        OnGunChange?.Invoke(hasGun);
    }

    public void SetHasJetPack(bool hasJetPack)
    {
        _hasJetPack = hasJetPack;
        OnJetPackChange?.Invoke(hasJetPack);
    }

    public bool GetCanShoot() { return _hasGun; }
    public bool GetCanFly() { return _hasJetPack; }
    
    public void SetCurrentJetPackFuel(float fuel)
    {
        _currentJetPackFuel = fuel;
    }

    public float GetMaxFuel()
    {
        return _currentJetPackFuel;
    }
}