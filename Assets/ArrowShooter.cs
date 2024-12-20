using UnityEngine;
using UnityEngine.UI;

public class ArrowShooter : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;  // Assign your arrow UI prefab
    [SerializeField] private float arrowSpeed = 300f; // Adjust speed as needed
    [SerializeField] private Transform gridContainer; // Reference to your Grid/Cells container
    
    private GameObject currentArrow;
    private Transform arrowRect;
    private bool isShooting = false;
    public bool puzzleComplete = false;
    private GridLayoutGroup grid;
    private float cellSize;

    void Start()
    {
        grid = transform.Find("Grid").GetComponent<GridLayoutGroup>();
        cellSize = grid.cellSize.y;
        SpawnArrow();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isShooting && !puzzleComplete)
        {
            isShooting = true;
        }

        if (isShooting && currentArrow != null)
        {
            // Move arrow upward
            arrowRect.Translate(Vector3.up * arrowSpeed * Time.deltaTime);

            // Check collisions
            CheckCollisions();
        }
    }

    private void SpawnArrow()
    {
        // Calculate spawn position (middle of bottom row)
        DestroyArrow();

        currentArrow = Instantiate(arrowPrefab, transform);
        arrowRect = currentArrow.GetComponent<Transform>();

        // Position the arrow at the bottom middle of the grid
        // Vector2 gridSize = grid.cellSize;
        // float startX = (gridSize.x * 2); // Middle of a 4x4 grid
        // float startY = -(gridSize.y * 5); // Bottom of the grid
        
        // arrowRect.anchoredPosition = new Vector2(startX, startY);
        isShooting = false;
    }

	private void DestroyArrow()
	{
		if (currentArrow != null)
		{
			Destroy(currentArrow);
		}
	}

    private void CheckCollisions()
    {
        // Cast a ray upward from the arrow's position
        RaycastHit2D[] hits = Physics2D.RaycastAll(arrowRect.position, Vector2.up, 0.1f);
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                // Check if we hit a wall (Fixed block)
                if (hit.collider.CompareTag("Puzzle2-Walls"))
                {
					puzzleComplete = true;
                    return;
                }
                // If we hit any other type of block, just reset
                else if (hit.collider.CompareTag("Puzzle2-Movable"))
                {
					SpawnArrow();
                    return;
                }
            }
        }

        // Check if arrow has gone off screen
        // if (arrowRect.anchoredPosition.y > cellSize * 5) // Adjust based on your grid size
        // {
        //     SpawnArrow();
        // }
    }
}