using UnityEngine;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{
    [Tooltip("Size of each grid cell in world units.")]
    public float cellSize = 1.0f;

    [Tooltip("Maximum distance from the center to fill the grid.")]
    public float maxDistance = 20f;

    [Tooltip("Prefab for the grid square (must have GridSquareWireframe attached).")]
    public GameObject gridSquarePrefab;

    [Tooltip("Margin for collision checking on grid squares.")]
    public float margin = 0.05f;

    public GameControllerVision gameController;

    public Dictionary<Vector2Int, GameObject> gridSquares = new Dictionary<Vector2Int, GameObject>();
    

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> processed = new HashSet<Vector2Int>();
        
        Vector2Int[] initialCells = new Vector2Int[]
        {
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 0)
        };

        foreach (Vector2Int cellCoord in initialCells)
        {
            queue.Enqueue(cellCoord);
            processed.Add(cellCoord);
        }

        while (queue.Count > 0)
        {
            Vector2Int cellCoord = queue.Dequeue();
            Vector3 cellCenter = transform.position + new Vector3(cellCoord.x * cellSize + cellSize / 2f, 0, cellCoord.y * cellSize + cellSize / 2f);

            if (Vector3.Distance(transform.position, cellCenter) > maxDistance)
                continue;

            Vector3 halfExtents = new Vector3(cellSize / 2f - margin, 1f, cellSize / 2f - margin);
            Collider[] hits = Physics.OverlapBox(cellCenter, halfExtents, Quaternion.identity);
            bool blocked = false;
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == gameObject)
                    continue;
                if (hit.CompareTag("Obstacle") || hit.CompareTag("Bed"))
                {
                    blocked = true;
                    break;
                }
            }

            if (!blocked)
            {
                if (gridSquarePrefab != null)
                {
                    GameObject square = Instantiate(gridSquarePrefab, cellCenter, Quaternion.identity, transform);
                    square.name = $"Square_{cellCoord.x}_{cellCoord.y}";

                    GridSquare ws = square.GetComponent<GridSquare>();
                    if (ws != null)
                        ws.gridCoordinate = cellCoord;

                    gridSquares.Add(cellCoord, square);
                }

                Vector2Int[] neighbors = new Vector2Int[]
                {
                    new Vector2Int(cellCoord.x - 1, cellCoord.y),
                    new Vector2Int(cellCoord.x + 1, cellCoord.y),
                    new Vector2Int(cellCoord.x, cellCoord.y - 1),
                    new Vector2Int(cellCoord.x, cellCoord.y + 1)
                };

                foreach (Vector2Int neighbor in neighbors)
                {
                    if (!processed.Contains(neighbor))
                    {
                        processed.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
        
        gameController.SetUpGame();
    }
}
