using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    private int totalObjectives;
    private int collectedObjectives = 0;
    private Objective[] allObjectives;

    void Start()
    {
        InitializeObjectives();
    }

    private void InitializeObjectives()
    {
        // Find all objectives in the scene
        allObjectives = FindObjectsByType<Objective>(FindObjectsSortMode.None);
        totalObjectives = allObjectives.Length;
        Debug.Log("Total objectives: " + totalObjectives);
        // Subscribe to each objective's onCollected event
        foreach (var objective in allObjectives)
        {
            objective.onCollected.AddListener(OnObjectiveCollected);
        }
    }

    public void ResetObjectives()
    {
        // Reset counter
        collectedObjectives = 0;
        
        // Find and reactivate all objectives
        foreach (var objective in allObjectives)
        {
            if (objective != null)
            {
                objective.gameObject.SetActive(true);
            }
        }
        
        Debug.Log("Objectives reset. Total objectives: " + totalObjectives);
    }

    public bool IsAllObjectivesCollected()
    {
        return collectedObjectives >= totalObjectives;
    }

    private void OnObjectiveCollected()
    {
        collectedObjectives++;
    }
}