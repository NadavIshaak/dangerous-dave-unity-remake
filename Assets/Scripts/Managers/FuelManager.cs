using System;

namespace Managers
{
    public class FuelManager
    {
        private float _currentJetPackFuel;
        public bool HasJetPack;

        public FuelManager(float fuel)
        {
            _currentJetPackFuel = fuel;
            HasJetPack = false;
        }

        public event Action<bool> OnJetPackChange;
        public event Action<float, float> OnFuelChange;

        public void SetHasJetPack(bool hasJetPack)
        {
            HasJetPack = hasJetPack;
            if(hasJetPack)
                _currentJetPackFuel = 100f;
            OnJetPackChange?.Invoke(hasJetPack);
        }

        public bool GetCanFly()
        {
            return HasJetPack;
        }

        public void SetCurrentJetPackFuel(float fuel)
        {
            _currentJetPackFuel = fuel;
            
        }

        public float GetMaxFuel()
        {
            return _currentJetPackFuel;
        }

        public void UpdateFuelBar(float currentFuel, float maxFuel)
        {
            OnFuelChange?.Invoke(currentFuel, maxFuel);
        }
    }
}