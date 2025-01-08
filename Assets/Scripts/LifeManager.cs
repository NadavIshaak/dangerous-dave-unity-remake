using UnityEngine;
using UnityEngine.UI;
public class LifeManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Image _oneLifeRenderer;
    [SerializeField] Image _twoLifeRenderer;
    [SerializeField] Image _threeLifeRenderer;
    private int _lives = 4;
     public void RemoveLife()
    {
        _lives--;
        UpdateLifeDisplay();
    }
    private void UpdateLifeDisplay()
    {
        switch (_lives)
        {
            case 3:
                _threeLifeRenderer.enabled = false; break;
            case 2:
                _twoLifeRenderer.enabled = false; break;
            case 1:
                _oneLifeRenderer.enabled = false; break;
        }
    }
    
}
