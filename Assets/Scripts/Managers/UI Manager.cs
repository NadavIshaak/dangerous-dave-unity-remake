using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [FormerlySerializedAs("_numberSprites")] [SerializeField] private Sprite[] numberSprites;

    [FormerlySerializedAs("_tenOfThousandsRenderer")] [SerializeField] private Image tenOfThousandsRenderer;
    [FormerlySerializedAs("_thousandsRenderer")] [SerializeField] private Image thousandsRenderer;
    [FormerlySerializedAs("_hundredsRenderer")] [SerializeField] private Image hundredsRenderer;
    [FormerlySerializedAs("_tensRenderer")] [SerializeField] private Image tensRenderer;
    [FormerlySerializedAs("_onesRenderer")] [SerializeField] private Image onesRenderer;
    [FormerlySerializedAs("_oneLifeRenderer")] [SerializeField] private Image oneLifeRenderer;
    [FormerlySerializedAs("_twoLifeRenderer")] [SerializeField] private Image twoLifeRenderer;
    [FormerlySerializedAs("_threeLifeRenderer")] [SerializeField] private Image threeLifeRenderer;
    [FormerlySerializedAs("_DeadRenderer")] [SerializeField] private Image deadRenderer;
    [SerializeField] private Image fuelBar; // The full fuel bar
    [SerializeField] private Image blackBox; // The black box that indicates fuel depletion
    [FormerlySerializedAs("_GunSymbolRenderer")] [SerializeField] private Image gunSymbolRenderer;
    [FormerlySerializedAs("_GunTextRenderer")] [SerializeField] private Image gunTextRenderer;
    [FormerlySerializedAs("_JetPackTextRenderer")] [SerializeField] private Image jetPackTextRenderer;
    [FormerlySerializedAs("_trophyCollectedRenderer")] [SerializeField] private Image trophyCollectedRenderer;
    [FormerlySerializedAs("_LevelOnesRenderer")] [SerializeField] private Image levelOnesRenderer;
    [FormerlySerializedAs("_LevelTensRenderer")] [SerializeField] private Image levelTensRenderer;

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
    
}
