using Unity.Cinemachine;

namespace Managers
{
    using System;
    using UnityEngine;

    public class PlayerManager
    {
        public bool HasGun;
        private int _playerHealth;
        private readonly GameObject _player;
        private readonly GameObject[] _stagesSpawns;
        private readonly CinemachineSplineCart[] _dollyCart;
        private readonly CurrentLevelManagar _currentLevelManagar;
        public event Action OnInstantiatedPlayer;
        public event Action<int> OnLifeChange; 
        public event Action<bool> OnGunChange;
        public PlayerManager(GameObject player, GameObject[] stagesSpawns, CinemachineSplineCart[] dollyCart, CurrentLevelManagar currentLevelManagar)
        {
            _player = player;
            _stagesSpawns = stagesSpawns;
            _dollyCart = dollyCart;
            _currentLevelManagar = currentLevelManagar;
            _playerHealth = 4;
        }
        
        public void InstantiatePlayer()
        {
            var currentLevel = _currentLevelManagar.LevelManager.CurrentLevel;
            if (_playerHealth==0)
            {
                return;
            }
            var spawnPosition = _stagesSpawns[currentLevel].transform.position;
            if (currentLevel is 2 or 3) _dollyCart[currentLevel - 2].SplinePosition = 0;
            UnityEngine.Object.Instantiate(_player, spawnPosition, Quaternion.identity);
            OnInstantiatedPlayer?.Invoke();
        }
        public void TriggerPlayerDeath()
        {
            _playerHealth--;
            OnLifeChange?.Invoke(_playerHealth);
            var player = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
            if (player is null) return;
            player.TriggerDeath();
            _currentLevelManagar.Invoke(nameof(InstantiatePlayer), 3.1f);
        }
        public void SetHasGun(bool hasGun)
        {
            HasGun = hasGun;
            OnGunChange?.Invoke(hasGun);
        }
        public bool GetCanShoot() { return HasGun; }
    }
    
}