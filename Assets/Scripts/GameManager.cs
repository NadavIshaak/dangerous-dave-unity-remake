using UnityEngine;
using UnityEngine.UI;

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
    private int _score = 0;
    private int _lives = 3;

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
    private void Start()
    {
    }
     private void AssignUIElements()
    {
        _tenOfThousandsRenderer = GameObject.Find("TensOfThousands")?.GetComponent<Image>();
        _thousandsRenderer = GameObject.Find("Thousands")?.GetComponent<Image>();
        _hundredsRenderer = GameObject.Find("Hundreds")?.GetComponent<Image>();
        _tensRenderer = GameObject.Find("Tens")?.GetComponent<Image>();
        _onesRenderer = GameObject.Find("Ones")?.GetComponent<Image>();
        _oneLifeRenderer = GameObject.Find("OneLife")?.GetComponent<Image>();
        _twoLifeRenderer = GameObject.Find("TwoLife")?.GetComponent<Image>();
        _threeLifeRenderer = GameObject.Find("ThreeLife")?.GetComponent<Image>();
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
        if (_oneLifeRenderer == null || _twoLifeRenderer == null || _threeLifeRenderer == null)
        {
            AssignUIElements();
        }

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
        if (_tenOfThousandsRenderer == null || _thousandsRenderer == null || _hundredsRenderer == null || _tensRenderer == null || _onesRenderer == null)
        {
            AssignUIElements();
        }
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
}