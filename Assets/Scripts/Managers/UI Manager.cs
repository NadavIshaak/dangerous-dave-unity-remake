using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Cache = UnityEngine.Cache;

public class UIManager : MonoBehaviour
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

   

    private void Start()
    {
        CurrentLevelManagar.Instance.LevelManager.OnLevelChange += UpdateLevel;
        CurrentLevelManagar.Instance.LevelManager.OnTrophyChange += UpdateThrophy;
        CurrentLevelManagar.Instance.PlayerManager.OnGunChange += GotGun;
        CurrentLevelManagar.Instance.FuelManager.OnJetPackChange += GotJetPack;
        CurrentLevelManagar.Instance.FuelManager.OnFuelChange += UpdateFuelBar;
        CurrentLevelManagar.Instance.OnShowTriggerText += SetText;
        CurrentLevelManagar.Instance.PlayerManager.OnLifeChange += UpdateLife;
        CurrentLevelManagar.Instance.ScoreManager.OnScoreChange += UpdateScore;
    }

    private void OnDisable()
    {
        CurrentLevelManagar.Instance.LevelManager.OnLevelChange -= UpdateLevel;
        CurrentLevelManagar.Instance.LevelManager.OnTrophyChange -= UpdateThrophy;
        CurrentLevelManagar.Instance.PlayerManager.OnGunChange -= GotGun;
        CurrentLevelManagar.Instance.FuelManager.OnJetPackChange -= GotJetPack;
        CurrentLevelManagar.Instance.FuelManager.OnFuelChange -= UpdateFuelBar;
        CurrentLevelManagar.Instance.OnShowTriggerText -= SetText;
        CurrentLevelManagar.Instance.PlayerManager.OnLifeChange -= UpdateLife;
        CurrentLevelManagar.Instance.ScoreManager.OnScoreChange -= UpdateScore;
    }

    private void UpdateThrophy(bool trophyCollected)
    {
        trophyCollectedRenderer.enabled = trophyCollected;
    }

    private void UpdateFuelBar(float currentFuel, float maxFuel)
    {
        var fuelPercentage = currentFuel / maxFuel;
        var blackBoxWidth = fuelBar.rectTransform.rect.width * (1 - fuelPercentage);
        blackBox.rectTransform.pivot = new Vector2(1, 0.5f); // Set pivot to the right
        blackBox.rectTransform.sizeDelta = new Vector2(blackBoxWidth, blackBox.rectTransform.sizeDelta.y);
    }

    private void OnPlayerFinalDeath()
    {
        gunSymbolRenderer.enabled = false;
        gunTextRenderer.enabled = false;
        jetPackTextRenderer.enabled = false;
        trophyCollectedRenderer.enabled = false;
        blackBox.enabled = false;
        fuelBar.enabled = false;
        deadRenderer.enabled = true;
    }

    private void UpdateLevel(int level)
    {
        var tens = level / 10;
        var ones = level % 10;

        levelTensRenderer.sprite = numberSprites[tens];
        levelOnesRenderer.sprite = numberSprites[ones];
    }

    private void GotGun(bool hasGun)
    {
        gunSymbolRenderer.enabled = hasGun;
        gunTextRenderer.enabled = hasGun;
    }

    private void GotJetPack(bool hasJetPack)
    {
        jetPackTextRenderer.enabled = hasJetPack;
        blackBox.enabled = hasJetPack;
        fuelBar.enabled = hasJetPack;
    }

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

    private void SetText(string text,bool active)
    {
        messageText.text = text;
        //messageText.gameObject.SetActive(active);
        if (!active) return;
        messageText.rectTransform.DOShakePosition(1f, new Vector3(0.5f, 0, 0), 5, 90, false, true);
        talkingDave.rectTransform.DOShakePosition(1f, new Vector3(0.5f, 0, 0), 5, 90, false, true);
    }
   
}
