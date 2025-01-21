using System;
using Triggers;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CurrentLevelManagar : MonoBehaviour
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
    private bool _trophyCollected;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the SoundManager persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public event Action OnVictoryWalkStart;
    public event Action OnInstantiatedPlayer;
    public event Action OnGameOver;
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
            requirement.RequiredLevel==_currentLevel)
        {
            UIManager.Instance.SetText(message,true);
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
    private void HideText()
    {
        UIManager.Instance.SetText("",false);
    }
    public void ThrophyCollected()
    {
        UIManager.Instance.UpdateThrophy(true);
        _trophyCollected = true;
    }

    public void UpdateFuelBar(float currentFuel, float maxFuel)
    {
        UIManager.Instance.UpdateFuelBar(currentFuel, maxFuel);
    }

    public void DoorReached()
    {
        if (!_trophyCollected) return;
        _trophyCollected = false;
        _StageWinWalkRenderer.sprite = _StageWinWalkSprite[_currentLevel - 1];
        _hasGun = false;
        _hasJetPack = false;
        UIManager.Instance.UpdateThrophy(false);
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

        UpdateLevel();
        InstantiatePlayer();
    }

    public void InstantiatePlayer()
    {
        if (LifeManager.Instance.GetLife() == 0)
        {
            UIManager.Instance.OnPlayerFinalDeath();
            return;
        }

        var spawnPosition = _stagesSpawns[_currentLevel].transform.position;
        if (_currentLevel is 2 or 3) dollyCart[_currentLevel - 2].SplinePosition = 0;
        Instantiate(Player, spawnPosition, Quaternion.identity);
        Debug.Log("revived");
        OnInstantiatedPlayer?.Invoke();
    }

    private void UpdateLevel()
    {
        UIManager.Instance.UpdateLevel(_currentLevel);
    }

    public void TriggerPlayerDeath()
    {
        var player = FindFirstObjectByType<PlayerMovement>();
        if (player is null) return;
        player.TriggerDeath();
        Invoke(nameof(InstantiatePlayer), 3.1f);
    }

    public void SetHasGun(bool hasGun)
    {
        _hasGun = hasGun;
        UIManager.Instance.GotGun(hasGun);
    }

    public void SetHasJetPack(bool hasJetPack)
    {
        _hasJetPack = hasJetPack;
        UIManager.Instance.GotJetPack(hasJetPack);
    }

    public bool GetCanShoot() { return _hasGun; }
    public bool GetCanFly() { return _hasJetPack; }
}