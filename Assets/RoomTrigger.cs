using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("Room Light Settings")]
    public Light roomLight;      // This should be your HDRP area light
    public Light globalLight;    // E.g., a directional light to disable
    public Transform checkpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable the global directional light and enable the room's area light
            if (globalLight != null)
                globalLight.enabled = false;
            
            if (roomLight != null)
                roomLight.enabled = true;

            // Let the PlayerController know about this room
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetCurrentRoom(this);
            }
        }
    }
}
