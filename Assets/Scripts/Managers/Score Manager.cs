using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    private int _score;
    public void AddScore(int value)
    {
        _score += value;
        UIManager.Instance.UpdateScore(_score);
    }
}