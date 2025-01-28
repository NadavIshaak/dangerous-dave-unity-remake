using UnityEngine;

/**
 * This class is responsible for handling the door object.
 */
public class DoorBehavior : MonoBehaviour
{
    /**
     * Check if the player has collided with the door.
     * if the player has collided with the door,
     * call the DoorReached method from the CurrentLevelManager class.
     */
    private void OnCollisionEnter2D(Collision2D other)
    {
        CurrentLevelManagar.instance.DoorReached();
    }
}