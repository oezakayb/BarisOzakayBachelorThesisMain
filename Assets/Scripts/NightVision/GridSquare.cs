using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GridSquare : MonoBehaviour
{
    [Tooltip("Size of this grid cell (width and height).")]
    public float cellSize = 1.0f;

    [Tooltip("Edge color (default is green).")]
    public Color edgeColor = Color.green;

    [Tooltip("Width of the wireframe lines.")]
    public float lineWidth = 0.05f;

    [Tooltip("Margin for collision checking on this grid square.")]
    public float collisionMargin = 0.05f;
    
    private LineRenderer topEdge;
    private LineRenderer bottomEdge;
    private LineRenderer leftEdge;
    private LineRenderer rightEdge;
    
    public Vector2Int gridCoordinate;

    void Start()
    {
        CreateEdges();
        CreateCollider();
        CheckForObstacles();
    }

    void CreateEdges()
    {
        topEdge = CreateEdge("TopEdge", new Vector3(-cellSize / 2f, 0, cellSize / 2f), new Vector3(cellSize / 2f, 0, cellSize / 2f), edgeColor);
        bottomEdge = CreateEdge("BottomEdge", new Vector3(-cellSize / 2f, 0, -cellSize / 2f), new Vector3(cellSize / 2f, 0, -cellSize / 2f), edgeColor);
        leftEdge = CreateEdge("LeftEdge", new Vector3(-cellSize / 2f, 0, -cellSize / 2f), new Vector3(-cellSize / 2f, 0, cellSize / 2f), edgeColor);
        rightEdge = CreateEdge("RightEdge", new Vector3(cellSize / 2f, 0, -cellSize / 2f), new Vector3(cellSize / 2f, 0, cellSize / 2f), edgeColor);
    }

    LineRenderer CreateEdge(string edgeName, Vector3 start, Vector3 end, Color color)
    {
        GameObject edgeObj = new GameObject(edgeName);
        edgeObj.transform.parent = transform;
        edgeObj.transform.localPosition = Vector3.zero;
        LineRenderer lr = edgeObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        return lr;
    }

    void CreateCollider()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc == null)
        {
            bc = gameObject.AddComponent<BoxCollider>();
        }
        bc.center = Vector3.zero;
        bc.size = new Vector3(cellSize, 0.1f, cellSize);
    }
    
    void CheckForObstacles()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        Vector3 halfExtents = (bc.size / 2f) - new Vector3(collisionMargin, 0, collisionMargin);
        Collider[] hits = Physics.OverlapBox(transform.position, halfExtents, Quaternion.identity);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;
            if (hit.CompareTag("Obstacle"))
            {
                Destroy(gameObject);
                return;
            }
        }
    }
    
    public void SetEdgeColor(Color newColor)
    {
        edgeColor = newColor;
        if (topEdge != null)
        {
            topEdge.startColor = newColor;
            topEdge.endColor = newColor;
        }
        if (bottomEdge != null)
        {
            bottomEdge.startColor = newColor;
            bottomEdge.endColor = newColor;
        }
        if (leftEdge != null)
        {
            leftEdge.startColor = newColor;
            leftEdge.endColor = newColor;
        }
        if (rightEdge != null)
        {
            rightEdge.startColor = newColor;
            rightEdge.endColor = newColor;
        }
    }
}
