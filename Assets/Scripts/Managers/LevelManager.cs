namespace Managers
{
    using System;
    using UnityEngine;

    public class LevelManager
    {
        private readonly SpriteRenderer _stageWinWalkRenderer;
        private readonly Sprite[] _stageWinWalkSprites;
        private readonly CurrentLevelManagar _currentLevelManager;
        private int _currentLevel = 1;

        public event Action OnVictoryWalkStart;
        public event Action OnVictoryWalkEnd;
        public event Action<int> OnLevelChange;
        public event Action OnGameOver;

        public LevelManager(SpriteRenderer stageWinWalkRenderer, Sprite[] stageWinWalkSprites, CurrentLevelManagar currentLevelManager)
        {
            this._stageWinWalkRenderer = stageWinWalkRenderer;
            this._stageWinWalkSprites = stageWinWalkSprites;
            this._currentLevelManager = currentLevelManager;
        }

        public void DoorReached()
        {
            _stageWinWalkRenderer.sprite = _stageWinWalkSprites[_currentLevel - 1];
            OnVictoryWalkStart?.Invoke();
        }

        public void OnVictoryWalkEnded()
        {
            _currentLevel++;
            if (_currentLevel == 4)
            {
                OnGameOver?.Invoke();
                return;
            }
            OnLevelChange?.Invoke(_currentLevel);
            _currentLevelManager.InstantiatePlayer();
        }
    }
}