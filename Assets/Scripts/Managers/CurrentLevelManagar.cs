using System;
using Managers;
using Triggers;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * This class is responsible for managing the current level.
 * It is used to manage the player, fuel, level, and score managers.
 */
public class CurrentLevelManagar : MonoBehaviour
{
    public static CurrentLevelManagar instance;

    [FormerlySerializedAs("Player")] [SerializeField]
    private GameObject player;

    [FormerlySerializedAs("_stagesSpawns")] [SerializeField]
    private GameObject[] stagesSpawns;

    [FormerlySerializedAs("_StageWinWalkRenderer")] [SerializeField]
    private SpriteRenderer stageWinWalkRenderer;

    [FormerlySerializedAs("_StageWinWalkSprite")] [SerializeField]
    private Sprite[] stageWinWalkSprite;

    [SerializeField] private CinemachineSplineCart[] dollyCart;
    public FuelManager FuelManager;
    public LevelManager LevelManager;
    public PlayerManager PlayerManager;
    public ScoreManager ScoreManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the SoundManager persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to both events
        OnShowTriggerTextWithReq += ShowTextWithRequirement;
        OnHideTriggerText += HideText;
        PlayerManager = new PlayerManager(player, stagesSpawns, dollyCart, this);
        LevelManager = new LevelManager(this, stageWinWalkRenderer, stageWinWalkSprite);
        FuelManager = new FuelManager(100);
        ScoreManager = new ScoreManager();
    }

    private void OnDisable()
    {
        OnShowTriggerTextWithReq -= ShowTextWithRequirement;
        OnHideTriggerText -= HideText;
        CancelInvoke();
    }

    private void OnDestroy()
    {
        OnShowTriggerTextWithReq -= ShowTextWithRequirement;
        OnHideTriggerText -= HideText;
        CancelInvoke();
    }

    public event Action<string, bool> OnShowTriggerText;
    public static event Action<string, TriggerRequirements> OnShowTriggerTextWithReq;
    public static event Action OnHideTriggerText;

    /**
     * Show the trigger text if the requirements are met
     */
    private void ShowTextWithRequirement(string message, TriggerRequirements requirement)
    {
        if (requirement.HasGun == PlayerManager.HasGun &&
            requirement.HasJetPack == FuelManager.HasJetPack &&
            requirement.RequiredScore < ScoreManager.Score &&
            requirement.RequiredLevel == LevelManager.CurrentLevel
            && requirement.HasTrophy == LevelManager.TrophyCollected)
            OnShowTriggerText?.Invoke(message, true);
    }
    //Public class methods to be used by game objectives:

    public static void HideTriggerText()
    {
        OnHideTriggerText?.Invoke();
    }

    public static void ShowTriggerText(string message, TriggerRequirements requirement)
    {
        OnShowTriggerTextWithReq?.Invoke(message, requirement);
    }

    private void HideText()
    {
        OnShowTriggerText?.Invoke("", false);
    }

    public void ThrophyCollected()
    {
        LevelManager.ThrophyCollected();
    }

    public void UpdateFuelBar(float currentFuel, float maxFuel)
    {
        FuelManager.UpdateFuelBar(currentFuel, maxFuel);
    }

    public void DoorReached()
    {
        LevelManager.DoorReached();
    }

    public void OnVictoryWalkEnd()
    {
        LevelManager.OnVictoryWalkEnd();
    }

    public void TriggerPlayerDeath()
    {
        PlayerManager.TriggerPlayerDeath();
    }

    public void SetHasGun(bool hasGun)
    {
        PlayerManager.SetHasGun(hasGun);
    }

    public void SetHasJetPack(bool hasJetPack)
    {
        FuelManager.SetHasJetPack(hasJetPack);
    }

    public bool GetCanShoot()
    {
        return PlayerManager.GetCanShoot();
    }

    public bool GetCanFly()
    {
        return FuelManager.GetCanFly();
    }

    public void SetCurrentJetPackFuel(float fuel)
    {
        FuelManager.SetCurrentJetPackFuel(fuel);
    }

    public float GetMaxFuel()
    {
        return FuelManager.GetMaxFuel();
    }

    public void InstantiatePlayer()
    {
        PlayerManager.InstantiatePlayer();
    }

    public void AddScore(int score)
    {
        ScoreManager.AddScore(score);
    }
}