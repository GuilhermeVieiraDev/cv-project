using UnityEngine;
using System.Collections;

public class RotationPuzzleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform immobile;
    [SerializeField] private Transform ball;

    [Header("Settings")]
    [SerializeField] private float rotationDuration = 0.25f;

    private bool isRotating = false;

    void Update()
    {
        if (!isRotating)
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

    private IEnumerator RotateAllSquares(float angle)
    {
        isRotating = true;

        // Disable gravity
        Rigidbody2D ballRigidbody = ball.GetComponent<Rigidbody2D>();
        float previousGravityScale = ballRigidbody.gravityScale;
        if (ballRigidbody != null)
        {
            ballRigidbody.gravityScale = 0f;
        }

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

        yield return new WaitForSeconds(1f);

        // Enable gravity
        if (ballRigidbody != null)
        {
            ballRigidbody.gravityScale = previousGravityScale;
        }

        isRotating = false;
    }
}
