using Triggers;
using UnityEngine;
/**
 * This class is responsible for handling the level triggers, they require the level maker to
 * set up  requirements for the player to trigger the event and show a message.
 */
[RequireComponent(typeof(Collider2D))]
public class LevelTrigger : MonoBehaviour
{
    [TextArea] [SerializeField] private string messageToShow;

    [SerializeField] private bool hasGun;
    [SerializeField] private bool hasJetPack;
    [SerializeField] private bool hasTrophy;
    [SerializeField] private int requiredScore;
    [SerializeField] private int requiredLevel;
    [SerializeField] private bool hideOnExit = true; // if true, hide text on exit

    private TriggerRequirements RequirementToShow =>
        new(messageToShow, hasGun, hasJetPack, requiredScore, requiredLevel, hasTrophy);

    /**
     * When the player enters the trigger, show the message
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if 'other' is the player
        if (other.CompareTag("Player"))
            // Fire the event with BOTH the message and the requirement
            // The LevelManager listens to this event
            CurrentLevelManagar.ShowTriggerText(messageToShow, RequirementToShow);
    }

    /**
     * When the player exits the trigger, hide the message
     */
    private void OnTriggerExit2D(Collider2D other)
    {
        if (hideOnExit && other.CompareTag("Player"))
            // Fire the hide event
            CurrentLevelManagar.HideTriggerText();
    }
}