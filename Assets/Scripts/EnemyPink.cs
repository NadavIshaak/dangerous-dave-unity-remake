using UnityEngine;

public class EnemyPink : MonoBehaviour
{
    private Vector3 fixedRotation = new Vector3(0, 0, 0); // Fixed rotation value
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}
