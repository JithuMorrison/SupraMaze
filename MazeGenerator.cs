using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject wallPrefab;
    public GameObject startPrefab;
    public GameObject endPrefab;
    public float cellSize = 1.0f;

    private int[,] maze; // 0 = open, 1 = wall
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // Fill the maze with walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1;
            }
        }

        // Carve the maze using DFS
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = new Vector2Int(1, 1);
        maze[start.x, start.y] = 0;
        stack.Push(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                maze[next.x, next.y] = 0;
                maze[(current.x + next.x) / 2, (current.y + next.y) / 2] = 0; // Carve a path between
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }

        // Ensure end position is accessible and connected to the maze
        Vector2Int end = new Vector2Int(width - 2, height - 2);
        maze[end.x, end.y] = 0; // Make the end cell open

        // Try to ensure the end node is part of the maze path
        EnsureEndPath(end);
    }

    void EnsureEndPath(Vector2Int end)
    {
        // Ensure the end node is connected by carving out the wall between it and the nearest open space.
        // If it's not directly connected to the path, we create a path from a nearby open cell.
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
            Vector2Int neighbor = cell + dir * 2; // Check 2 steps away
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
        // Create walls and place the start and end points
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);

                if (maze[x, y] == 1)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
                else if (x == 1 && y == 1)
                {
                    Instantiate(startPrefab, position, Quaternion.identity, transform);
                }
                else if (x == width - 2 && y == height - 2)
                {
                    Instantiate(endPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}
