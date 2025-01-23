using System;

namespace Managers
{
    public class ScoreManager
    {
        public int Score = 0;
        public Action<int> OnScoreChange;

        public void AddScore(int value)
        {
            Score += value;
            OnScoreChange?.Invoke(Score);
        }
    }
}