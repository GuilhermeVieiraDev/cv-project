using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    private int totalObjectives;
    private int collectedObjectives = 0;
    private Objective[] allObjectives;

    void Start()
    {
        // Find all objectives in the scene
        allObjectives = FindObjectsByType<Objective>(FindObjectsSortMode.None);
        totalObjectives = allObjectives.Length;
        
        // Subscribe to each objective's onCollected event
        foreach (var objective in allObjectives)
        {
            objective.onCollected.AddListener(OnObjectiveCollected);
        }
    }

    private void OnObjectiveCollected()
    {
        collectedObjectives++;
        
        if (collectedObjectives >= totalObjectives)
        {
            Debug.Log("All objectives collected!");
            // Add your level complete logic here
        }
    }
}