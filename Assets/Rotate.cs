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

    void Start()
    {
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isRotating && !IsBallFalling())
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(RotateAllSquares(-90f)); // Counter-clockwise
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
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
