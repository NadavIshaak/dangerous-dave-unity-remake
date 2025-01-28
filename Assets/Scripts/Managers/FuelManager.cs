using System;

namespace Managers
{
    /**
     * Fuel manager class that handles the jetpack fuel
     */
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

        /**
         * Set the player's jetpack status, if the player has the jetpack, set the fuel to 100
         * and trigger the jetpack change event
         */
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

        /**
         * Update the fuel bar in the ui with the current fuel and max fuel
         */
        public void UpdateFuelBar(float currentFuel, float maxFuel)
        {
            OnFuelChange?.Invoke(currentFuel, maxFuel);
        }
    }
}