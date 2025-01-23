namespace Managers
{
    using System;
    using UnityEngine;

    public class LevelManager
    {
        private readonly CurrentLevelManagar _currentLevelManagar;
        public int CurrentLevel = 1;
        public bool _trophyCollected;
        private readonly SpriteRenderer _stageWinWalkRenderer;
        private readonly Sprite[] _stageWinWalkSprite;
        public event Action OnGameOver;
        public event Action<int> OnLevelChange;
        public event Action<bool> OnTrophyChange; 
        public event Action OnVictoryWalkStart;
        public LevelManager(CurrentLevelManagar currentLevelManagar, SpriteRenderer stageWinWalkRenderer, Sprite[] stageWinWalkSprite)
        {
            _currentLevelManagar = currentLevelManagar;
            _stageWinWalkRenderer = stageWinWalkRenderer;
            _stageWinWalkSprite = stageWinWalkSprite;
        }
        public void OnVictoryWalkEnd()
        {
            // Handle the end of the victory walk
            // Example: Spawn the player in the next area
            _currentLevelManagar.SetHasGun(false);
            _currentLevelManagar.FuelManager.SetHasJetPack(false);
            CurrentLevel++;
            if (CurrentLevel == 4)
            {
                OnGameOver?.Invoke();
                return;
            }
            OnLevelChange?.Invoke(CurrentLevel);
            _currentLevelManagar.InstantiatePlayer();
        }
        public void ThrophyCollected()
        {
            OnTrophyChange?.Invoke(true);
            _trophyCollected = true;
        }
        public void DoorReached()
        {
            if (!_trophyCollected) return;
            _trophyCollected = false;
            _stageWinWalkRenderer.sprite = _stageWinWalkSprite[_currentLevelManagar.LevelManager.CurrentLevel - 1];
            _currentLevelManagar.PlayerManager.HasGun = false;
            _currentLevelManagar.FuelManager.HasJetPack = false;
            OnTrophyChange?.Invoke(false);
            OnVictoryWalkStart?.Invoke();
        }
    }
}