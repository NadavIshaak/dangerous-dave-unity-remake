namespace Managers
{
    using System;

    public class FuelManager
    {
        public bool HasJetPack;
        private float _currentJetPackFuel;
        public event Action<bool> OnJetPackChange;
        public event Action<float,float> OnFuelChange;

        public FuelManager(float fuel)
        {
            _currentJetPackFuel = fuel;
            HasJetPack = false;
        }
        public void SetHasJetPack(bool hasJetPack)
        {
            HasJetPack = hasJetPack;
            OnJetPackChange?.Invoke(hasJetPack);
        }
        public bool GetCanFly() { return HasJetPack; }
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
            OnFuelChange?.Invoke(currentFuel,maxFuel);
        }
        }
}
