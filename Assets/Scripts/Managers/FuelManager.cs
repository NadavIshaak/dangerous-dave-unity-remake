namespace Managers
{
    using System;

    public class FuelManager
    {
        private readonly CurrentLevelManagar _currentLevelManager;
        private float _currentJetPackFuel;

        public event Action<float, float> OnFuelChange;

        public FuelManager(CurrentLevelManagar currentLevelManager)
        {
            _currentLevelManager = currentLevelManager;
        }

        public void UpdateFuelBar(float currentFuel, float maxFuel)
        {
            OnFuelChange?.Invoke(currentFuel, maxFuel);
        }

        public void SetCurrentJetPackFuel(float fuel)
        {
            _currentJetPackFuel = fuel;
        }

        public float GetMaxFuel()
        {
            return _currentJetPackFuel;
        }
    }
}