using UnityEngine;
using System.Collections;

public class RotationPuzzleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform immobile;
    [SerializeField] private Transform ball;

    [Header("Settings")]
    [SerializeField] private float rotationDuration = 0.25f;
    [SerializeField] private float velocityThreshold = 0.1f;

    private bool isRotating = false;
    private Rigidbody2D ballRigidbody;
    private ObjectiveManager objectiveManager;

    // Store initial states
    private Vector3 initialBallPosition;
    private Quaternion initialImmobileRotation;
    private Vector2 initialBallVelocity = Vector2.zero;

    public bool isPuzzleSolved = false;

    void Start()
    {
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
        objectiveManager = GetComponent<ObjectiveManager>();

        if (objectiveManager == null)
        {
            objectiveManager = gameObject.AddComponent<ObjectiveManager>();
        }

        // Store initial states when the game starts
        StoreInitialState();
    }

    private void StoreInitialState()
    {
        initialBallPosition = ball.position;
        initialImmobileRotation = immobile.rotation;
    }

    public void ResetToInitialState()
    {
        // Stop any ongoing rotation coroutine
        StopAllCoroutines();
        isRotating = false;

        // Reset positions and rotations
        immobile.rotation = initialImmobileRotation;
        ball.position = initialBallPosition;

        // Reset ball physics
        if (ballRigidbody != null)
        {
            ballRigidbody.linearVelocity = initialBallVelocity;
            ballRigidbody.angularVelocity = 0f;
        }

        ResetObjectives();

        isPuzzleSolved = false;
    }

    private void ResetObjectives()
    {
        if (objectiveManager != null)
        {
            objectiveManager.ResetObjectives();
        }
        else
        {
            // Fallback if objectiveManager is missing
            Objective[] objectives = FindObjectsByType<Objective>(FindObjectsSortMode.None);
            foreach (var objective in objectives)
            {
                if (objective != null)
                {
                    objective.gameObject.SetActive(true);
                }
            }
        }
    }

    void Update()
    {
        // Add reset functionality
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToInitialState();
            return;
        }

        if (objectiveManager != null && objectiveManager.IsAllObjectivesCollected())
        {
            isPuzzleSolved = true;
        }

        if (!isRotating && !IsBallFalling())
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartCoroutine(RotateAllSquares(-90f)); // Counter-clockwise
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(RotateAllSquares(90f)); // Clockwise
            }
        }
    }

    private bool IsBallFalling()
    {
        if (ballRigidbody != null)
        {
            // Check if vertical velocity is significant
            return Mathf.Abs(ballRigidbody.linearVelocity.y) > velocityThreshold;
        }
        return false;
    }

    private IEnumerator RotateAllSquares(float angle)
    {
        isRotating = true;

        // Disable gravity
        float previousGravityScale = ballRigidbody.gravityScale;
        ballRigidbody.gravityScale = 0f;

        float elapsed = 0f;

        // Store initial rotations
        Quaternion initialRotation = immobile.rotation;
        Quaternion targetRotation = immobile.rotation * Quaternion.Euler(0, 0, angle);

        // Smooth rotation
        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;

            immobile.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);

            yield return null;
        }

        immobile.rotation = targetRotation;

        yield return new WaitForSeconds(0.1f); // Reduced wait time for better responsiveness

        // Restore gravity
        ballRigidbody.gravityScale = previousGravityScale;
        isRotating = false;
    }
}