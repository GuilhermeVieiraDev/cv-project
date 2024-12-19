using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FlexibleBlockMovement : MonoBehaviour
{
    private GridLayoutGroup gridLayout;
    private RectTransform[] gridCells;
    private int gridSize = 5;
    private bool isAnimating = false;
    [SerializeField] private float moveSpeed = 5f;

    void Start()
    {
        Transform cellsContainer = transform.Find("Grid");
        if (cellsContainer == null)
        {
            Debug.LogError("Could not find Cells container!");
            return;
        }

        gridLayout = cellsContainer.GetComponent<GridLayoutGroup>();
        
        gridCells = new RectTransform[cellsContainer.childCount];
        for (int i = 0; i < cellsContainer.childCount; i++)
        {
            gridCells[i] = cellsContainer.GetChild(i) as RectTransform;
        }
    }

    void Update()
    {
        if (isAnimating) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveFlexibleBlocks(Vector2Int.down);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveFlexibleBlocks(Vector2Int.up);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveFlexibleBlocks(Vector2Int.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveFlexibleBlocks(Vector2Int.right);
    }

    private void MoveFlexibleBlocks(Vector2Int direction)
    {
        bool reverse = direction.x > 0 || direction.y > 0;
        List<int> movesToMake = new List<int>();

        // First pass: identify all moves that need to be made
        for (int i = reverse ? gridCells.Length - 1 : 0; 
             reverse ? i >= 0 : i < gridCells.Length; 
             i += reverse ? -1 : 1)
        {
            if (gridCells[i].CompareTag("Puzzle2-Movable"))
            {
                List<int> chainMoves = CheckChainMove(i, direction);
                if (chainMoves.Count > 0)
                {
                    movesToMake.AddRange(chainMoves);
                }
            }
        }

        // Second pass: execute all moves
        if (movesToMake.Count > 0)
        {
            StartCoroutine(AnimateChainMoves(movesToMake, direction));
        }
    }

    private List<int> CheckChainMove(int startIndex, Vector2Int direction)
    {
        List<int> chainMoves = new List<int>();
        int currentRow = startIndex / gridSize;
        int currentCol = startIndex % gridSize;
        
        // Check backwards in the chain first
        List<int> flexibleChain = new List<int>();
        int checkRow = currentRow;
        int checkCol = currentCol;
        
        // Build the chain of flexible blocks
        while (true)
        {
            int currentIndex = checkRow * gridSize + checkCol;
            if (currentIndex >= 0 && currentIndex < gridCells.Length && 
                gridCells[currentIndex].CompareTag("Puzzle2-Movable"))
            {
                flexibleChain.Add(currentIndex);
                // Move check position opposite to movement direction
                checkRow -= direction.y;
                checkCol -= direction.x;
                
                // Check if still in bounds
                if (checkRow < 0 || checkRow >= gridSize || 
                    checkCol < 0 || checkCol >= gridSize)
                    break;
            }
            else
                break;
        }

        // Now check if there's an empty space after the last flexible block in the chain
        int targetRow = currentRow + direction.y;
        int targetCol = currentCol + direction.x;
        int targetIndex = targetRow * gridSize + targetCol;

        if (targetRow >= 0 && targetRow < gridSize && 
            targetCol >= 0 && targetCol < gridSize && 
            targetIndex >= 0 && targetIndex < gridCells.Length)
        {
            if (gridCells[targetIndex].CompareTag("Puzzle2-Empty"))
            {
                // Add all blocks in the chain to moves
                chainMoves.AddRange(flexibleChain);
            }
        }

        return chainMoves;
    }

    private IEnumerator AnimateChainMoves(List<int> indices, Vector2Int direction)
    {
        isAnimating = true;
        gridLayout.enabled = false;

        // Sort indices based on direction to prevent blocking
        if (direction.x > 0 || direction.y > 0)
        {
            indices.Sort((a, b) => b.CompareTo(a));  // Reverse order for right/down
        }
        else
        {
            indices.Sort();  // Normal order for left/up
        }

        // Store initial positions
        Dictionary<int, Vector3> startPositions = new Dictionary<int, Vector3>();
        Dictionary<int, Vector3> targetPositions = new Dictionary<int, Vector3>();

        foreach (int index in indices)
        {
            int targetRow = index / gridSize + direction.y;
            int targetCol = index % gridSize + direction.x;
            int targetIndex = targetRow * gridSize + targetCol;

            startPositions[index] = gridCells[index].position;
            targetPositions[index] = gridCells[targetIndex].position;
        }

        // Animate movement
        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * moveSpeed;
            float t = Mathf.SmoothStep(0, 1, elapsedTime);

            foreach (int index in indices)
            {
                gridCells[index].position = Vector3.Lerp(
                    startPositions[index], 
                    targetPositions[index], 
                    t
                );
            }

            yield return null;
        }

        // Finalize moves
        foreach (int index in indices)
        {
            int targetRow = index / gridSize + direction.y;
            int targetCol = index % gridSize + direction.x;
            int targetIndex = targetRow * gridSize + targetCol;

            // Swap the blocks
            int index1Sibling = gridCells[index].GetSiblingIndex();
            int index2Sibling = gridCells[targetIndex].GetSiblingIndex();
            
            RectTransform temp = gridCells[index];
            gridCells[index] = gridCells[targetIndex];
            gridCells[targetIndex] = temp;
            
            gridCells[index].SetSiblingIndex(index1Sibling);
            gridCells[targetIndex].SetSiblingIndex(index2Sibling);
        }

        gridLayout.enabled = true;
        isAnimating = false;
    }
}