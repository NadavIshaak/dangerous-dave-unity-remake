using UnityEngine;
using UnityEngine.UI;
using System; 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] Sprite[] _numberSprites;
    [SerializeField] Image _trophyCollectedRenderer;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _LevelOnesRenderer;
    [SerializeField] private Image _LevelTensRenderer;
    [SerializeField] private GameObject Player;
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
            _canvas.enabled = false;
            OnVictoryWalkStart?.Invoke();
        }
    }
    public void OnVictoryWalkEnd()
    {
        // Handle the end of the victory walk
        // Example: Spawn the player in the next area
        _canvas.enabled = true;
        _currentLevel++;
        updateLevel();
        Vector3 nextAreaPosition = new Vector3(20, 0, 0); // Example position
        Instantiate(Player, nextAreaPosition, Quaternion.identity);
    }
    private void updateLevel()
    {
        int tens = _currentLevel / 10;
        int ones = _currentLevel % 10;

        _LevelTensRenderer.sprite = _numberSprites[tens];
        _LevelOnesRenderer.sprite = _numberSprites[ones];
    }

}