using System;

namespace Managers
{
    /**
     * Score manager class that handles the score of the player
     */
    public class ScoreManager
    {
        public Action<int> OnScoreChange;
        public int Score;

        public void AddScore(int value)
        {
            Score += value;
            OnScoreChange?.Invoke(Score);
        }
    }
}