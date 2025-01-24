using System;

namespace Managers
{
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