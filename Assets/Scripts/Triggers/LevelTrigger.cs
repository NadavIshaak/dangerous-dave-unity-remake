using Triggers;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class LevelTrigger : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string messageToShow;
    [SerializeField] private bool hasGun;
    [SerializeField] private bool hasJetPack;
    [SerializeField] private int requiredScore;
    [SerializeField] private int requiredLevel;
     private TriggerRequirements RequirementToShow=> new (messageToShow, hasGun, hasJetPack, requiredScore, requiredLevel);
    [SerializeField] private bool hideOnExit = true; // if true, hide text on exit

    private void Awake()
    {
     
       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if 'other' is the player
        if (other.CompareTag("Player"))
        {
            // Fire the event with BOTH the message and the requirement
            // The LevelManager listens to this event
            CurrentLevelManagar.ShowTriggerText(messageToShow, RequirementToShow);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (hideOnExit && other.CompareTag("Player"))
        {
            // Fire the hide event
            CurrentLevelManagar.HideTriggerText();
        }
    }
}

