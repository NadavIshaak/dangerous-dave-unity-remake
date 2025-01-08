using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int _value = 100;
    [SerializeField] private AudioClip _collectSound;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        SoundManager.Instance.PlaySound(_collectSound, transform, 1);
        ScoreManager.Instance.AddScore(_value);
        Destroy(gameObject);
    }
}
