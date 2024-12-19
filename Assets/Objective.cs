using UnityEngine;
using UnityEngine.Events;

public class Objective : MonoBehaviour
{
    public UnityEvent onCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puzzle1-Main"))
        {
            // Trigger any events set up in the Inspector
            onCollected?.Invoke();

            // Disable the objective
            gameObject.SetActive(false);
        }
    }
}
