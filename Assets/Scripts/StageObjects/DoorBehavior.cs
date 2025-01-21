using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameManager.instance.DoorReached();
    }
}