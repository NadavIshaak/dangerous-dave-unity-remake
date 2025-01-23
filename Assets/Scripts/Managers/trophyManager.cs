namespace Managers
{
    using System;

    public class TrophyManager
    {
        private bool _trophyCollected;

        public event Action<bool> OnTrophyChange;

        public void ThrophyCollected()
        {
            _trophyCollected = true;
            OnTrophyChange?.Invoke(true);
        }

        public void ResetTrophy()
        {
            _trophyCollected = false;
            OnTrophyChange?.Invoke(false);
        }
    }
}