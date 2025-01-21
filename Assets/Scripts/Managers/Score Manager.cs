using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    [SerializeField] private Sprite[] _numberSprites;

    [SerializeField] private Image _tenOfThousandsRenderer;
    [SerializeField] private Image _thousandsRenderer;
    [SerializeField] private Image _hundredsRenderer;
    [SerializeField] private Image _tensRenderer;
    [SerializeField] private Image _onesRenderer;
    private int _score;

    public void AddScore(int value)
    {
        _score += value;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        var tenThousands = _score / 10000;
        var thousands = _score % 10000 / 1000;
        var hundreds = _score % 1000 / 100;
        var tens = _score % 100 / 10;
        var ones = _score % 10;

        _tenOfThousandsRenderer.sprite = _numberSprites[tenThousands];
        _thousandsRenderer.sprite = _numberSprites[thousands];
        _hundredsRenderer.sprite = _numberSprites[hundreds];
        _tensRenderer.sprite = _numberSprites[tens];
        _onesRenderer.sprite = _numberSprites[ones];
    }
}