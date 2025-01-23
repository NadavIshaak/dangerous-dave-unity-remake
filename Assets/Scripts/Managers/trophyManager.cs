namespace Managers
{
    using System;

    public class TrophyManager
    {
        private bool trophyCollected;

        public event Action<bool> OnTrophyChange;

        public void ThrophyCollected()
        {
            trophyCollected = true;
            OnTrophyChange?.Invoke(true);
        }

        public void ResetTrophy()
        {
            trophyCollected = false;
            OnTrophyChange?.Invoke(false);
        }
    }
}