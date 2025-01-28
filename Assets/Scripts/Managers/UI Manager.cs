using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
/**
 * This class is responsible for managing the UI elements in the game.
 * It updates the UI elements based on the player's actions.
 */
public class UIManager : MonoBehaviour
{
    [FormerlySerializedAs("_numberSprites")] [SerializeField]
    private Sprite[] numberSprites;

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


    private void Start()
    {
        CurrentLevelManagar.instance.LevelManager.OnLevelChange += UpdateLevel;
        CurrentLevelManagar.instance.LevelManager.OnTrophyChange += UpdateThrophy;
        CurrentLevelManagar.instance.PlayerManager.OnGunChange += GotGun;
        CurrentLevelManagar.instance.FuelManager.OnJetPackChange += GotJetPack;
        CurrentLevelManagar.instance.FuelManager.OnFuelChange += UpdateFuelBar;
        CurrentLevelManagar.instance.OnShowTriggerText += SetText;
        CurrentLevelManagar.instance.PlayerManager.OnLifeChange += UpdateLife;
        CurrentLevelManagar.instance.ScoreManager.OnScoreChange += UpdateScore;
    }

    private void OnDisable()
    {
        CurrentLevelManagar.instance.LevelManager.OnLevelChange -= UpdateLevel;
        CurrentLevelManagar.instance.LevelManager.OnTrophyChange -= UpdateThrophy;
        CurrentLevelManagar.instance.PlayerManager.OnGunChange -= GotGun;
        CurrentLevelManagar.instance.FuelManager.OnJetPackChange -= GotJetPack;
        CurrentLevelManagar.instance.FuelManager.OnFuelChange -= UpdateFuelBar;
        CurrentLevelManagar.instance.OnShowTriggerText -= SetText;
        CurrentLevelManagar.instance.PlayerManager.OnLifeChange -= UpdateLife;
        CurrentLevelManagar.instance.ScoreManager.OnScoreChange -= UpdateScore;
    }

    /**
     * Update the throphy sentence on the screen to the throphy we get
     */
    private void UpdateThrophy(bool trophyCollected)
    {
        trophyCollectedRenderer.enabled = trophyCollected;
    }

    /**
     * Update the fuel bar on the screen to the fuel we get
     */
    private void UpdateFuelBar(float currentFuel, float maxFuel)
    {
        var fuelPercentage = currentFuel / maxFuel;
        var blackBoxWidth = fuelBar.rectTransform.rect.width * (1 - fuelPercentage);
        blackBox.rectTransform.pivot = new Vector2(1, 0.5f); // Set pivot to the right
        blackBox.rectTransform.sizeDelta = new Vector2(blackBoxWidth, blackBox.rectTransform.sizeDelta.y);
    }

    /**
     * When the player dies for the
     * last time, show the dead screen
     * and disable everything else in the ui
     */
    private void OnPlayerFinalDeath()
    {
        gunSymbolRenderer.enabled = false;
        gunTextRenderer.enabled = false;
        jetPackTextRenderer.enabled = false;
        trophyCollectedRenderer.enabled = false;
        blackBox.enabled = false;
        fuelBar.enabled = false;
        talkingDave.enabled = false;
        messageText.enabled = false;
        deadRenderer.enabled = true;
    }

    /**
     * Update the level on the screen to the level we get
     */
    private void UpdateLevel(int level)
    {
        var tens = level / 10;
        var ones = level % 10;

        levelTensRenderer.sprite = numberSprites[tens];
        levelOnesRenderer.sprite = numberSprites[ones];
    }

    /**
     * Make the gun symbol and text appear or disappear on the screen
     */
    private void GotGun(bool hasGun)
    {
        gunSymbolRenderer.enabled = hasGun;
        gunTextRenderer.enabled = hasGun;
    }

    /**
     * Make the jetpack words and bar appear or disappear on the screen
     */
    private void GotJetPack(bool hasJetPack)
    {
        jetPackTextRenderer.enabled = hasJetPack;
        blackBox.enabled = hasJetPack;
        fuelBar.enabled = hasJetPack;
    }

    /**
     * Update the life on the screen to the lives we get,make the icon dissapear
     */
    private void UpdateLife(int lives)
    {
        switch (lives)
        {
            case 3:
                threeLifeRenderer.enabled = false; break;
            case 2:
                twoLifeRenderer.enabled = false; break;
            case 1:
                oneLifeRenderer.enabled = false; break;
            case 0:
                OnPlayerFinalDeath(); break;
        }
    }

    /**
     * Update the score on the screen to the score we get
     */
    private void UpdateScore(int score)
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

    /**
     * show the message text on the screen and make it and ui save shake
     */
    private void SetText(string text, bool active)
    {
        messageText.text = text;
        if (!active) return;
        messageText.rectTransform.DOShakePosition(1f, new Vector3(0.5f, 0, 0), 5);
        talkingDave.rectTransform.DOShakePosition(1f, new Vector3(0.5f, 0, 0), 5);
    }
}