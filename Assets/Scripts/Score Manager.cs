using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoSingleton<ScoreManager>
{
    private int _score = 0;
     [SerializeField] Sprite[] _numberSprites;

    [SerializeField] Image _tenOfThousandsRenderer;
    [SerializeField] Image _thousandsRenderer;
    [SerializeField] Image _hundredsRenderer;
    [SerializeField] Image _tensRenderer;
    [SerializeField] Image _onesRenderer;
     public void AddScore(int value)
    {
        _score += value;
        UpdateScoreDisplay();
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
}
