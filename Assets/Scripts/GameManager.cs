using UnityEngine;
using UnityEngine.UI;
using System; 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] Sprite[] _numberSprites;

    [SerializeField] Image _tenOfThousandsRenderer;
    [SerializeField] Image _thousandsRenderer;
    [SerializeField] Image _hundredsRenderer;
    [SerializeField] Image _tensRenderer;
    [SerializeField] Image _onesRenderer;
    [SerializeField] Image _oneLifeRenderer;
    [SerializeField] Image _twoLifeRenderer;
    [SerializeField] Image _threeLifeRenderer;
    [SerializeField] Image _trophyCollectedRenderer;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _LevelOnesRenderer;
    [SerializeField] private Image _LevelTensRenderer;
    [SerializeField] private GameObject Player;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera victoryWalkCamera;
    bool _trophyCollected = false;
     public event Action OnVictoryWalkStart;

    private int _score = 0;
    private int _lives = 3;
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
    public void AddScore(int value)
    {
        _score += value;
        UpdateScoreDisplay();
    }
    public void RemoveLife()
    {
        _lives--;
        UpdateLifeDisplay();
    }
    private void UpdateLifeDisplay()
    {
        switch (_lives)
        {
            case 2:
                _threeLifeRenderer.enabled = false; break;
            case 1:
                _twoLifeRenderer.enabled = false; break;
            case 0:
                _oneLifeRenderer.enabled = false; break;
        }
    }

    private void UpdateScoreDisplay()
    {
        int tenThousands = _score / 10000;
        int thousands = (_score % 10000) / 1000;
        int hundreds = (_score % 1000) / 100;
        int tens = (_score % 100) / 10;
        int ones = _score % 10;

        _tenOfThousandsRenderer.sprite = _numberSprites[tenThousands];
        _thousandsRenderer.sprite = _numberSprites[thousands];
        _hundredsRenderer.sprite = _numberSprites[hundreds];
        _tensRenderer.sprite = _numberSprites[tens];
        _onesRenderer.sprite = _numberSprites[ones];
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
            mainCamera.Priority = 0;
            victoryWalkCamera.Priority = 10;
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
        mainCamera.Priority = 10;
        victoryWalkCamera.Priority = 0;
    }
    private void updateLevel()
    {
        int tens = _currentLevel / 10;
        int ones = _currentLevel % 10;

        _LevelTensRenderer.sprite = _numberSprites[tens];
        _LevelOnesRenderer.sprite = _numberSprites[ones];
    }

}