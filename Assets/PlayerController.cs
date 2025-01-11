using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform head;
    public Camera camera;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpSpeed = 5f;

    [Header("Shadow Detection")]
    public float shadowCheckInterval = 0.1f;
    public float lightThreshold = 0.5f;
    public LayerMask shadowTestLayers;
    public float floorCheckDistance = 2f;  // How far down to check for floor
    private float nextShadowCheck;
    private bool shadowRuleActive = false;
    private Light currentRoomLight;
    private Transform currentCheckpoint;

    [Header("Runtime")]
    Vector3 newVelocity;
    bool isGrounded = false;
    bool isJumping = false;

    [Header("Light Sampling")]
    public Vector3[] samplingOffsets = new Vector3[]
    {
        Vector3.zero,              // Center
        new Vector3(0.3f, 0, 0),   // Right
        new Vector3(-0.3f, 0, 0),  // Left
        new Vector3(0, 0, 0.3f),   // Forward
        new Vector3(0, 0, -0.3f)   // Back
    };

    [Header("Debug")]
    public bool showDebug = true;
    public Color debugSafeColor = Color.green;
    public Color debugDangerColor = Color.red;
    private bool isInShadow = true;
    private Vector3 lastFloorPoint;
    private float lastLightIntensity;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Shadow checking
        if (shadowRuleActive && Time.time >= nextShadowCheck)
        {
            if (!CheckIfInShadow())
            {
                TeleportToCheckpoint();
                return;
            }
            nextShadowCheck = Time.time + shadowCheckInterval;
        }

        // Mouse look - horizontal rotation
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 2f);

        // Movement
        newVelocity = Vector3.up * rb.linearVelocity.y;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Horizontal movement (A and D keys)
        if (Input.GetKey(KeyCode.A))
        {
            newVelocity.x = -speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            newVelocity.x = speed;
        }
        else
        {
            newVelocity.x = 0f;
        }

        // Forward/backward movement (W and S keys)
        if (Input.GetKey(KeyCode.W))
        {
            newVelocity.z = speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            newVelocity.z = -speed;
        }
        else
        {
            newVelocity.z = 0f;
        }

        // Jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            newVelocity.y = jumpSpeed;
            isJumping = true;
        }

        // Apply movement
        rb.linearVelocity = transform.TransformDirection(newVelocity);
    }

    void LateUpdate()
    {
        // Mouse look - vertical rotation
        Vector3 e = head.eulerAngles;
        e.x -= Input.GetAxis("Mouse Y") * 2f;
        e.x = RestrictAngle(e.x, -85f, 85f);
        head.eulerAngles = e;
    }

    void FixedUpdate()
    {
        // Ground check
        if (Physics.Raycast(camera.transform.position, Vector3.down, out RaycastHit hit, 1f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void SetCurrentRoom(RoomTrigger room)
    {
        if (room != null && room.roomLight != null)
        {
            currentRoomLight = room.roomLight;
            currentCheckpoint = room.checkpoint;
            shadowRuleActive = true;
        }
        else
        {
            shadowRuleActive = false;
            currentRoomLight = null;
            currentCheckpoint = null;
        }
    }

    bool CheckIfInShadow()
    {
        if (!isGrounded)
        {
            return true;
        }

        if (currentRoomLight == null)
        {
            isInShadow = true;
            return true;
        }

        // First, raycast down to find the floor
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit floorHit, floorCheckDistance, shadowTestLayers))
        {
            isInShadow = false;
            return false;
        }

        // Store the floor point for debugging
        lastFloorPoint = floorHit.point;

        // Important: Offset the start position slightly above the floor to avoid self-intersection
        Vector3 checkPoint = lastFloorPoint + (Vector3.up * 0.05f);

        // Check if there's anything blocking the light to this floor point
        Vector3 directionToLight = (currentRoomLight.transform.position - checkPoint).normalized;
        float distanceToLight = Vector3.Distance(checkPoint, currentRoomLight.transform.position);

        // Raycast from slightly above floor point to light
        if (Physics.Raycast(checkPoint, directionToLight, out RaycastHit shadowHit, distanceToLight, shadowTestLayers))
        {
            if (showDebug)
            {
                Debug.DrawLine(checkPoint, shadowHit.point, Color.green, shadowCheckInterval);
            }
            isInShadow = true;
            return true;
        }

        // If nothing is blocking, calculate light intensity
        float lightIntensity = CalculateLightIntensityAtPoint(checkPoint);
        lastLightIntensity = lightIntensity;

        if (showDebug)
        {
            Debug.DrawLine(checkPoint, currentRoomLight.transform.position,
                Color.Lerp(Color.green, Color.red, lightIntensity / lightThreshold),
                shadowCheckInterval);

            // Debug.Log($"Light intensity at point: {lightIntensity:F2}");
        }

        isInShadow = lightIntensity <= lightThreshold;
        return isInShadow;
    }

    float CalculateLightIntensityAtPoint(Vector3 point)
    {
        if (currentRoomLight == null) return 0f;

        float distance = Vector3.Distance(point, currentRoomLight.transform.position);
        float normalizedDistance = Mathf.Clamp01(distance / currentRoomLight.range);

        float attenuation;
        if (currentRoomLight.type == LightType.Rectangle)
        {
            attenuation = 1f - Mathf.Pow(normalizedDistance, 2);
        }
        else
        {
            attenuation = 1f / (1f + 25f * normalizedDistance * normalizedDistance);
        }

        float intensity = currentRoomLight.intensity * attenuation;

        return intensity;
    }

    void TeleportToCheckpoint()
    {
        if (currentCheckpoint != null)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = currentCheckpoint.position;
        }
    }

    public static float RestrictAngle(float angle, float angleMin, float angleMax)
    {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;

        if (angle > angleMax)
            angle = angleMax;
        else if (angle < angleMin)
            angle = angleMin;

        return angle;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
        isJumping = false;
    }
}