using Unity.Cinemachine;
using UnityEngine;
/**
 * This class is responsible for handling the screen boundaries.
 * It is used to move the camera to the next or previous screen when the player crosses the boundary.
 */
public class ScreenBoundary : MonoBehaviour
{
    [SerializeField] private int currentScreenIndex;
    [SerializeField] private CinemachineSplineCart dollyCart;
    private float _boundaryX; // The X position of this boundary in world space

    /**
     * Check if the player has crossed the boundary and move the camera to the next or previous screen
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var playerX = other.transform.position.x;
        _boundaryX = transform.position.x;
        if (playerX > _boundaryX)
            // The player crossed this boundary from left to right, so go to the NEXT screen
            dollyCart.SplinePosition = currentScreenIndex;
        else
            // The player crossed from right to left, so go to the PREVIOUS screen
            dollyCart.SplinePosition = currentScreenIndex + 1;
    }

    /**
     * Check if the player has crossed the boundary and move the camera to the next or previous screen
     */
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var playerX = other.transform.position.x;
        _boundaryX = transform.position.x;
        if (playerX > _boundaryX)
            // The player crossed this boundary from left to right, so go to the NEXT screen
            dollyCart.SplinePosition = currentScreenIndex + 1;
        else
            // The player crossed from right to left, so go to the PREVIOUS screen
            dollyCart.SplinePosition = currentScreenIndex;
    }
}