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
    bool _trophyCollected = false;
     public event Action OnVictoryWalkStart;
    private int _currentLevel = 1;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the GameManager persists across scenes
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
        if (_trophyCollected)
        {
            _trophyCollected = false;
            _StageWinWalkRenderer.sprite = _StageWinWalkSprite[_currentLevel-1];
            OnVictoryWalkStart?.Invoke();
        }
    }
    public void OnVictoryWalkEnd()
    {
        // Handle the end of the victory walk
        // Example: Spawn the player in the next area

        _currentLevel++;
        updateLevel();
        InstantiatePlayer();
    }
    public void InstantiatePlayer()
    {
        Vector3 spawnPosition = _stagesSpawns[_currentLevel].transform.position;
        Instantiate(Player, spawnPosition, Quaternion.identity);
        Debug.Log("Player Instantiated");
    }
    private void updateLevel()
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
            this.Invoke("InstantiatePlayer", 3f);
        }
    }
   
}