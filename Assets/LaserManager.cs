using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    public static LaserManager Instance;
    public LaserController[] lasers;
    public bool puzzleComplete = false;
    private int currentLaserIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActivateLaser(0);
    }

    public void OnLaserAligned(LaserController alignedLaser)
    {
        if (currentLaserIndex < lasers.Length - 1)
        {
            if (currentLaserIndex == lasers.Length - 2)
            {
                // Delay 500ms before setting the puzzle as complete
                StartCoroutine(CompletePuzzle());
            }

            currentLaserIndex++;
            ActivateLaser(currentLaserIndex);
        }
    }

    private void ActivateLaser(int index)
    {
        lasers[index].isActive = true;
    }

    private IEnumerator CompletePuzzle()
    {
        yield return new WaitForSeconds(0.5f);
        puzzleComplete = true;
        Debug.Log("Puzzle complete!");
    }
}

