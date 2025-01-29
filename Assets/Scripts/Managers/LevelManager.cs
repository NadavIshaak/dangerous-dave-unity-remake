using System;
using UnityEngine;

namespace Managers
{
    /**
     * Level manager class that handles the level changes and the trophy collection
     */
    public class LevelManager
    {
        private readonly CurrentLevelManagar _currentLevelManagar;
        private readonly SpriteRenderer _stageWinWalkRenderer;
        private readonly Sprite[] _stageWinWalkSprite;
        public int CurrentLevel = 1;
        public bool TrophyCollected;

        public LevelManager(CurrentLevelManagar currentLevelManagar, SpriteRenderer stageWinWalkRenderer,
            Sprite[] stageWinWalkSprite)
        {
            _currentLevelManagar = currentLevelManagar;
            _stageWinWalkRenderer = stageWinWalkRenderer;
            _stageWinWalkSprite = stageWinWalkSprite;
        }

        public event Action OnGameOver;
        public event Action<int> OnLevelChange;
        public event Action<bool> OnTrophyChange;
        public event Action OnVictoryWalkStart;

        /**
         * Handle the end of the victory walk,
         * spawn the player in the next area
         * make the player lose the gun and jetpack
         * and change the level, checks for game over
         */
        public void OnVictoryWalkEnd()
        {
            // Handle the end of the victory walk
            _currentLevelManagar.SetHasGun(false);
            _currentLevelManagar.FuelManager.SetHasJetPack(false);
            CurrentLevel++;
            if (CurrentLevel == 4)
            {
                OnGameOver?.Invoke();
                return;
            }

            OnLevelChange?.Invoke(CurrentLevel);
            _currentLevelManagar.SetCurrentJetPackFuel(100f);
            _currentLevelManagar.InstantiatePlayer();
        }

        /**
         * Collect the trophy, change the trophy collected flag,trigger ui change
         */
        public void ThrophyCollected()
        {
            OnTrophyChange?.Invoke(true);
            TrophyCollected = true;
        }

        /**
         * when reaching the door, check if the trophy was collected,
         * if yes move to next stage if not do nothing. disable the gun and jetpack
         */
        public void DoorReached()
        {
            if (!TrophyCollected) return;
            TrophyCollected = false;
            _stageWinWalkRenderer.sprite = _stageWinWalkSprite[_currentLevelManagar.LevelManager.CurrentLevel - 1];
            _currentLevelManagar.PlayerManager.HasGun = false;
            _currentLevelManagar.FuelManager.HasJetPack = false;
            OnTrophyChange?.Invoke(false);
            OnVictoryWalkStart?.Invoke();
        }
    }
}