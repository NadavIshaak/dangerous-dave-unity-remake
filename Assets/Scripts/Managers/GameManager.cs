using UnityEngine;
using UnityEngine.UI;
using System; 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] Sprite[] _numberSprites;
    [SerializeField] Image _trophyCollectedRenderer;
    [SerializeField] private Image _LevelOnesRenderer;
    [SerializeField] private Image _LevelTensRenderer;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject[] _stagesSpawns;
    [SerializeField] private SpriteRenderer _StageWinWalkRenderer;
     [SerializeField] private Sprite[] _StageWinWalkSprite;
     [SerializeField] private Image _GunSymbolRenderer;
     [SerializeField] private Image _GunTextRenderer;
     [SerializeField] private Image _JetPackTextRenderer;
     private bool _trophyCollected = false;
    private bool _hasGun = false;
     public event Action OnVictoryWalkStart;
     public event Action OnInstantiatedPlayer;
     public event Action OnGameOver;
    private int _currentLevel = 1;
    private bool _hasJetPack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the SoundManager persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ThrophyCollected()
    {
        _trophyCollectedRenderer.enabled = true;
        _trophyCollected = true;
    }
    public void DoorReached()
    {
        if (!_trophyCollected) return;
        _trophyCollected = false;
        _StageWinWalkRenderer.sprite = _StageWinWalkSprite[_currentLevel-1];
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
        Vector3 spawnPosition = _stagesSpawns[_currentLevel].transform.position;
        Instantiate(Player, spawnPosition, Quaternion.identity);
        OnInstantiatedPlayer?.Invoke();
    }
    private void UpdateLevel()
    {
        int tens = _currentLevel / 10;
        int ones = _currentLevel % 10;

        _LevelTensRenderer.sprite = _numberSprites[tens];
        _LevelOnesRenderer.sprite = _numberSprites[ones];
    }
    public void TriggerPlayerDeath()
    {
         PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            player.TriggerDeath();
            Invoke(nameof(InstantiatePlayer), 3f);
        }
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
    }

    public bool GetCanShoot()
    {
        return _hasGun;
    }
}