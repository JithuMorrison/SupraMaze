using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject wallPrefab;
    public GameObject startPrefab;
    public GameObject endPrefab;
    public GameObject floorPrefab;
    public float cellSize = 1.0f;

    private int[,] maze;
    private Vector2Int[] directions = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private List<GameObject> mazeObjects = new List<GameObject>();
    private GameObject currentStartObject;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    public void RestartMaze()
    {
        DestroyAllMazeObjects();
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // Initialize maze to all walls (1)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1;
            }
        }

        // Start at (1, 1)
        Vector2Int start = new Vector2Int(1, 1);
        maze[start.x, start.y] = 0;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                maze[next.x, next.y] = 0;
                maze[(current.x + next.x) / 2, (current.y + next.y) / 2] = 0;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }

        // Set end position at (width-2, height-2)
        Vector2Int end = new Vector2Int(width - 2, height - 2);
        maze[end.x, end.y] = 0;
        EnsureEndPath(end);
    }

    void EnsureEndPath(Vector2Int end)
    {
        List<Vector2Int> neighbors = GetUnvisitedNeighbors(end);

        if (neighbors.Count > 0)
        {
            Vector2Int pathCell = neighbors[Random.Range(0, neighbors.Count)];
            maze[(end.x + pathCell.x) / 2, (end.y + pathCell.y) / 2] = 0;
            maze[pathCell.x, pathCell.y] = 0;
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = cell + dir * 2;
            if (IsInBounds(neighbor) && maze[neighbor.x, neighbor.y] == 1)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x > 0 && pos.x < width - 1 && pos.y > 0 && pos.y < height - 1;
    }

    void DrawMaze()
    {
        // Floor
        Vector3 floorPosition = new Vector3((width - 1) * cellSize / 2, -0.1f, (height - 1) * cellSize / 2);
        Vector3 floorScale = new Vector3((width - 1) * cellSize, 1, (height - 1) * cellSize);

        GameObject floor = Instantiate(floorPrefab, floorPosition, Quaternion.identity, transform);
        floor.transform.localScale = floorScale;
        mazeObjects.Add(floor);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);

                if (maze[x, y] == 1)
                {
                    GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity, transform);
                    mazeObjects.Add(wall);
                }
                else if (x == 1 && y == 1)
                {
                    currentStartObject = Instantiate(startPrefab, position, Quaternion.identity, transform);
                    mazeObjects.Add(currentStartObject);
                }
                else if (x == width - 2 && y == height - 2)
                {
                    GameObject end = Instantiate(endPrefab, position, Quaternion.identity, transform);
                    mazeObjects.Add(end);
                }
            }
        }
    }

    void DestroyAllMazeObjects()
    {
        foreach (GameObject obj in mazeObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        mazeObjects.Clear();
    }

    public Vector3 GetStartWorldPosition()
    {
        return currentStartObject != null ? currentStartObject.transform.position : Vector3.zero;
    }
}
