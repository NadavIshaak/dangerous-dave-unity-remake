using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class ScreenBoundary : MonoBehaviour
{
     [SerializeField] private int currentScreenIndex;
     [SerializeField] private CinemachineSplineCart dollyCart;
    private float boundaryX; // The X position of this boundary in world space

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var playerX = other.transform.position.x;
        boundaryX = transform.position.x;
        if (playerX > boundaryX)
        {
            // The player crossed this boundary from left to right, so go to the NEXT screen
            dollyCart.SplinePosition = currentScreenIndex;
        }
        else
        {
            // The player crossed from right to left, so go to the PREVIOUS screen
            dollyCart.SplinePosition = currentScreenIndex + 1;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var playerX = other.transform.position.x;
        boundaryX = transform.position.x;
        if (playerX > boundaryX)
        {
            // The player crossed this boundary from left to right, so go to the NEXT screen
            dollyCart.SplinePosition = currentScreenIndex+1;
        }
        else
        {
            // The player crossed from right to left, so go to the PREVIOUS screen
            dollyCart.SplinePosition = currentScreenIndex;
        }
    }
    
}
