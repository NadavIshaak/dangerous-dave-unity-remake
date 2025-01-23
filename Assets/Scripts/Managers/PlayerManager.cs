namespace Managers
{
    using System;
    using UnityEngine;

    public class PlayerManager
    {
        private readonly GameObject playerPrefab;
        private readonly GameObject[] stagesSpawns;
        private int currentLife = 4;

        public event Action OnPlayerInstantiated;
        public event Action OnPlayerDeath;

        public PlayerManager(GameObject playerPrefab, GameObject[] stagesSpawns)
        {
            this.playerPrefab = playerPrefab;
            this.stagesSpawns = stagesSpawns;
        }

        public void InstantiatePlayer(int currentLevel)
        {
            if (currentLife == 0) return;
            var spawnPosition = stagesSpawns[currentLevel].transform.position;
            UnityEngine.Object.Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            OnPlayerInstantiated?.Invoke();
        }

        public void TriggerPlayerDeath()
        {
            currentLife--;
            OnPlayerDeath?.Invoke();
            var player = UnityEngine.Object.FindObjectOfType<PlayerMovement>();
            if (player != null)
            {
                player.TriggerDeath();
                UnityEngine.Object.FindObjectOfType<CurrentLevelManagar>().Invoke(nameof(InstantiatePlayer), 3.1f);
            }
        }
    }
}