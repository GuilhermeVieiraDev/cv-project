using UnityEngine;

public class LeverInteractionScript3 : MonoBehaviour
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

    [Header("Minigame Settings")]
    [Tooltip("Reference to the minigame canvas")]
    public GameObject minigameCanvas;
    
    [Tooltip("Reference to the main game camera")]
    public Camera mainCamera;
    
    [Tooltip("Reference to the minigame camera")]
    public Camera minigameCamera;

    [Tooltip("Reference to the global light in the scene")]
    public Light globalLight;
    
    // [Tooltip("Reference to the player's movement script")]
    // public MonoBehaviour playerMovementScript;

    [Header("Player Reference")]
    [Tooltip("Reference to the player transform")]
    public Transform playerTransform;

    // Private variables
    private bool isPlayerNearby = false;
    private bool isLeverActivated = false;
    private bool isMinigameActive = false;
    private LaserManager minigameController;
    private MonoBehaviour playerMovementScript;

    void Start()
    {
        // Validate references
        if (leverAnimator == null)
            leverAnimator = GetComponent<Animator>();

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Get reference to minigame controller
        if (minigameCanvas != null)
            minigameController = FindObjectOfType<LaserManager>();
            
        // Initial setup
        if (minigameCanvas != null)
            minigameCanvas.SetActive(false);
            
        if (minigameCamera != null)
            minigameCamera.gameObject.SetActive(false);

        playerMovementScript = playerTransform.GetComponent<MonoBehaviour>();

        ValidateReferences();
    }

    void ValidateReferences()
    {
        if (playerTransform == null)
            Debug.LogError("Player transform not assigned and could not be found!");
        if (leverAnimator == null)
            Debug.LogError("No Animator component found for the lever!");
        if (doorAnimator == null)
            Debug.LogError("No Animator component found for the door!");
        if (minigameCanvas == null)
            Debug.LogError("Minigame canvas not assigned!");
        if (mainCamera == null)
            Debug.LogError("Main camera not assigned!");
        if (minigameCamera == null)
            Debug.LogError("Minigame camera not assigned!");
        if (playerMovementScript == null)
            Debug.LogError("Player movement script not assigned!");
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (isMinigameActive)
        {
            HandleMinigameInput();
            return;
        }

        // Calculate distance between player and lever
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerNearby = distanceToPlayer <= interactionRadius;

        // Interaction logic
        if (isPlayerNearby && Input.GetKeyDown(interactionKey) && !isLeverActivated)
        {
            StartMinigame();

            if (minigameCanvas != null)
                minigameController = FindObjectOfType<LaserManager>();
        }
    }

    void StartMinigame()
    {
        globalLight.intensity = 5f;
        isMinigameActive = true;
        
        // Disable player movement
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // Switch cameras
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);
        if (minigameCamera != null)
            minigameCamera.gameObject.SetActive(true);

        // Show minigame UI
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(true);
            // Reset minigame to initial state
            // if (minigameController != null)
                // minigameController.ResetToInitialState();
        }

        // Optional: Pause the main game time
        Time.timeScale = 1f; // Keep at 1 if minigame needs regular time
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnMinigameCompleted()
    {
        EndMinigame();
        ActivateLever();
    }

    void EndMinigame()
    {
        globalLight.intensity = 0.5f;
        isMinigameActive = false;

        // Re-enable player movement
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        // Switch back cameras
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);
        if (minigameCamera != null)
            minigameCamera.gameObject.SetActive(false);

        // Hide minigame UI
        if (minigameCanvas != null)
            minigameCanvas.SetActive(false);

        // Restore time scale
        Time.timeScale = 1f;
        
        // Reset cursor state (assuming it was locked before)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void HandleMinigameInput()
    {
        // Add escape key to cancel minigame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndMinigame();
        }

        if (minigameController != null && minigameController.puzzleComplete)
        {
            OnMinigameCompleted();
            return;
        }
    }

    void ActivateLever()
    {
        // Trigger the lever animation
        if (leverAnimator != null)
            leverAnimator.SetTrigger(leverAnimationTriggerName);

        // Trigger the door animation
        if (doorAnimator != null)
            doorAnimator.SetTrigger(doorAnimationTriggerName);

        // Mark the lever as activated
        isLeverActivated = true;

        Debug.Log("Lever activated! Door animation triggered.");
    }
}