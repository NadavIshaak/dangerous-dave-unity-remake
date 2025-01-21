using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private Sprite[] _numberSprites;
    [SerializeField] private Image _trophyCollectedRenderer;
    [SerializeField] private Image _LevelOnesRenderer;
    [SerializeField] private Image _LevelTensRenderer;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject[] _stagesSpawns;
    [SerializeField] private SpriteRenderer _StageWinWalkRenderer;
    [SerializeField] private Sprite[] _StageWinWalkSprite;
    [SerializeField] private Image _GunSymbolRenderer;
    [SerializeField] private Image _GunTextRenderer;
    [SerializeField] private Image _JetPackTextRenderer;
    [SerializeField] private CinemachineSplineCart[] dollyCart;
    [SerializeField] private Image fuelBar; // The full fuel bar
    [SerializeField] private Image blackBox; // The black box that indicates fuel depletion
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

    public void ThrophyCollected()
    {
        _trophyCollectedRenderer.enabled = true;
        _trophyCollected = true;
    }

    public void UpdateFuelBar(float currentFuel, float maxFuel)
    {
        var fuelPercentage = currentFuel / maxFuel;
        var blackBoxWidth = fuelBar.rectTransform.rect.width * (1 - fuelPercentage);
        blackBox.rectTransform.pivot = new Vector2(1, 0.5f); // Set pivot to the right
        blackBox.rectTransform.sizeDelta = new Vector2(blackBoxWidth, blackBox.rectTransform.sizeDelta.y);
    }

    public void DoorReached()
    {
        if (!_trophyCollected) return;
        _trophyCollected = false;
        _StageWinWalkRenderer.sprite = _StageWinWalkSprite[_currentLevel - 1];
        _hasGun = false;
        _hasJetPack = false;
        _trophyCollectedRenderer.enabled = false;
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
            _GunSymbolRenderer.enabled = false;
            _GunTextRenderer.enabled = false;
            _JetPackTextRenderer.enabled = false;
            _trophyCollectedRenderer.enabled = false;
            blackBox.enabled = false;
            fuelBar.enabled = false;
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
        var tens = _currentLevel / 10;
        var ones = _currentLevel % 10;

        _LevelTensRenderer.sprite = _numberSprites[tens];
        _LevelOnesRenderer.sprite = _numberSprites[ones];
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
        _GunSymbolRenderer.enabled = hasGun;
        _GunTextRenderer.enabled = hasGun;
    }

    public void SetHasJetPack(bool hasJetPack)
    {
        _hasJetPack = hasJetPack;
        _JetPackTextRenderer.enabled = hasJetPack;
        blackBox.enabled = hasJetPack;
        fuelBar.enabled = hasJetPack;
    }

    public bool GetCanShoot()
    {
        return _hasGun;
    }
}