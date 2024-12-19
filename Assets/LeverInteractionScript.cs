using UnityEngine;

public class LeverInteractionScript : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Maximum distance to interact with the lever")]
    public float interactionRadius = 3f;

    [Tooltip("Key to press for interaction")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("Lever Animation Settings")]
    [Tooltip("Animator component for the lever")]
    public Animator leverAnimator;

    [Tooltip("Name of the trigger parameter in the Animator for the lever")]
    public string leverAnimationTriggerName = "Activate";

    [Header("Door Animation Settings")]
    [Tooltip("Animator component for the door")]
    public Animator doorAnimator;

    [Tooltip("Name of the trigger parameter in the Animator for the door")]
    public string doorAnimationTriggerName = "Open";

    [Header("Player Reference")]
    [Tooltip("Reference to the player transform")]
    public Transform playerTransform;

    // Private variables
    private bool isPlayerNearby = false;
    private bool isLeverActivated = false;

    void Start()
    {
        // Validate references
        if (leverAnimator == null)
        {
            leverAnimator = GetComponent<Animator>();
        }

        if (playerTransform == null)
        {
            // Attempt to find the player if not manually assigned
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Ensure we have a valid player and animator
        if (playerTransform == null)
        {
            Debug.LogError("Player transform not assigned and could not be found!");
        }

        if (leverAnimator == null)
        {
            Debug.LogError("No Animator component found for the lever!");
        }

        if (doorAnimator == null)
        {
            Debug.LogError("No Animator component found for the door!");
        }
    }

    void Update()
    {
        // Check if player reference exists
        if (playerTransform == null) return;

        // Calculate distance between player and lever
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Check if player is within interaction radius
        isPlayerNearby = distanceToPlayer <= interactionRadius;

        // Interaction logic
        if (isPlayerNearby && Input.GetKeyDown(interactionKey) && !isLeverActivated)
        {
            ActivateLever();
        }
    }

    void ActivateLever()
    {
        // Trigger the lever animation
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger(leverAnimationTriggerName);
        }

        // Trigger the door animation
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(doorAnimationTriggerName);
        }

        // Mark the lever as activated
        isLeverActivated = true;

        Debug.Log("Lever activated! Door animation triggered.");
    }
}
