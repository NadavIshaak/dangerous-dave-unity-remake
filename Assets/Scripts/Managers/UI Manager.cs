using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [FormerlySerializedAs("_numberSprites")] [SerializeField] private Sprite[] numberSprites;

    [SerializeField] private Image tenOfThousandsRenderer;
   [SerializeField] private Image thousandsRenderer;
   [SerializeField] private Image hundredsRenderer;
    [SerializeField] private Image tensRenderer;
   [SerializeField] private Image onesRenderer;
   [SerializeField] private Image oneLifeRenderer;
     [SerializeField] private Image twoLifeRenderer;
   [SerializeField] private Image threeLifeRenderer;
    [SerializeField] private Image deadRenderer;
    [SerializeField] private Image fuelBar; // The full fuel bar
    [SerializeField] private Image blackBox; // The black box that indicates fuel depletion
  [SerializeField] private Image gunSymbolRenderer;
    [SerializeField] private Image gunTextRenderer;
    [SerializeField] private Image jetPackTextRenderer;
  [SerializeField] private Image trophyCollectedRenderer;
 [SerializeField] private Image levelOnesRenderer;
   [SerializeField] private Image levelTensRenderer;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image talkingDave; 

    public void UpdateThrophy(bool trophyCollected)
    {
        trophyCollectedRenderer.enabled = trophyCollected;
    }
    public void UpdateFuelBar(float currentFuel, float maxFuel)
    {
        var fuelPercentage = currentFuel / maxFuel;
        var blackBoxWidth = fuelBar.rectTransform.rect.width * (1 - fuelPercentage);
        blackBox.rectTransform.pivot = new Vector2(1, 0.5f); // Set pivot to the right
        blackBox.rectTransform.sizeDelta = new Vector2(blackBoxWidth, blackBox.rectTransform.sizeDelta.y);
    }

    public void OnPlayerFinalDeath()
    {
        gunSymbolRenderer.enabled = false;
        gunTextRenderer.enabled = false;
        jetPackTextRenderer.enabled = false;
        trophyCollectedRenderer.enabled = false;
        blackBox.enabled = false;
        fuelBar.enabled = false;
        deadRenderer.enabled = true;
    }
    public void UpdateLevel(int level)
    {
        var tens = level / 10;
        var ones = level % 10;

        levelTensRenderer.sprite = numberSprites[tens];
        levelOnesRenderer.sprite = numberSprites[ones];
    }
    public void GotGun(bool hasGun)
    {
        gunSymbolRenderer.enabled = hasGun;
        gunTextRenderer.enabled = hasGun;
    }
    public void GotJetPack(bool hasJetPack)
    {
        jetPackTextRenderer.enabled = hasJetPack;
        blackBox.enabled = hasJetPack;
        fuelBar.enabled = hasJetPack;
    }

    public void UpdateLife(int lives)
    {
        switch (lives)
        {
            case 3:
                threeLifeRenderer.enabled = false; break;
            case 2:
                twoLifeRenderer.enabled = false; break;
            case 1:
                oneLifeRenderer.enabled = false; break;
        }
    }

    public void UpdateScore(int score)
    {
        var tenThousands = score / 10000;
        var thousands = score % 10000 / 1000;
        var hundreds = score % 1000 / 100;
        var tens = score % 100 / 10;
        var ones = score % 10;

        tenOfThousandsRenderer.sprite = numberSprites[tenThousands];
        thousandsRenderer.sprite = numberSprites[thousands];
        hundredsRenderer.sprite = numberSprites[hundreds];
        tensRenderer.sprite = numberSprites[tens];
        onesRenderer.sprite = numberSprites[ones];
    }
    public void SetText(string text,bool active)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(active);
        talkingDave.rectTransform.DOShakePosition(1f, new Vector3(0.5f, 0, 0), 5, 90, false, true);
        if (active)
            messageText.rectTransform.DOShakePosition(1f, new Vector3(0.5f, 0, 0), 5, 90, false, true);
    }
   
}
