using System;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Managers
{
    /**
     * Player manager class that handles the player's health and gun
     */
    public class PlayerManager
    {
        private readonly CurrentLevelManagar _currentLevelManagar;
        private readonly CinemachineSplineCart[] _dollyCart;
        private readonly GameObject _player;
        private readonly GameObject[] _stagesSpawns;
        private bool _isDead;
        private int _playerHealth;
        private GameObject _playerInstance;
        public bool HasGun;

        public PlayerManager(GameObject player, GameObject[] stagesSpawns, CinemachineSplineCart[] dollyCart,
            CurrentLevelManagar currentLevelManagar)
        {
            _player = player;
            _stagesSpawns = stagesSpawns;
            _dollyCart = dollyCart;
            _currentLevelManagar = currentLevelManagar;
            _playerHealth = 4;
        }

        public event Action OnInstantiatedPlayer;
        public event Action<int> OnLifeChange;
        public event Action<bool> OnGunChange;

        /**
         * Instantiate the player at the current level spawn point
         */
        public void InstantiatePlayer()
        {
            _isDead = false;
            var currentLevel = _currentLevelManagar.LevelManager.CurrentLevel;
            if (_playerHealth == 0) return;
            var spawnPosition = _stagesSpawns[currentLevel].transform.position;
            if (currentLevel is 2 or 3) _dollyCart[currentLevel - 2].SplinePosition = 0;
            _playerInstance = Object.Instantiate(_player, spawnPosition, Quaternion.identity);
            OnInstantiatedPlayer?.Invoke();
        }

        /**
         * Trigger the player's death
         */
        public void TriggerPlayerDeath()
        {
            if (_isDead) return;
            _isDead = true;
            _playerHealth--;
            OnLifeChange?.Invoke(_playerHealth);

            if (_playerInstance is null) return;
            _playerInstance.GetComponent<PlayerMovement>().TriggerDeath();
            _playerInstance = null;
            _currentLevelManagar.Invoke(nameof(InstantiatePlayer), 3.1f);
        }

        /**
         * Set the player's gun status
         */
        public void SetHasGun(bool hasGun)
        {
            HasGun = hasGun;
            OnGunChange?.Invoke(hasGun);
        }

        /**
         * Get the player's gun status
         */
        public bool GetCanShoot()
        {
            return HasGun;
        }
    }
}