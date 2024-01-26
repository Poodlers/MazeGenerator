using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepthFirstSearchNode
{
    public MazeCell Cell { get; set; }
    public DepthFirstSearchNode PreviousNode { get; set; } = null;

    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        DepthFirstSearchNode node = (DepthFirstSearchNode)obj;
        return (X == node.X) && (Z == node.Z);

    }

    // override object.GetHashCode
    public override int GetHashCode()
    {

        return base.GetHashCode();
    }

    public List<DepthFirstSearchNode> GetNeighbours(MazeCell[,] mazeGrid)
    {
        List<DepthFirstSearchNode> neighbours = new List<DepthFirstSearchNode>();
        if (X > 0 && mazeGrid[X - 1, Z].rightWall.activeSelf == false)
        {
            neighbours.Add(new DepthFirstSearchNode { Cell = mazeGrid[X - 1, Z], X = X - 1, Z = Z });
        }
        if (X < mazeGrid.GetLength(0) - 1 && mazeGrid[X + 1, Z].leftWall.activeSelf == false)
        {
            neighbours.Add(new DepthFirstSearchNode { Cell = mazeGrid[X + 1, Z], X = X + 1, Z = Z });
        }
        if (Z > 0 && mazeGrid[X, Z - 1].frontWall.activeSelf == false)
        {
            neighbours.Add(new DepthFirstSearchNode { Cell = mazeGrid[X, Z - 1], X = X, Z = Z - 1 });
        }
        if (Z < mazeGrid.GetLength(1) - 1 && mazeGrid[X, Z + 1].backWall.activeSelf == false)
        {
            neighbours.Add(new DepthFirstSearchNode { Cell = mazeGrid[X, Z + 1], X = X, Z = Z + 1 });
        }

        // display neighbours
        foreach (DepthFirstSearchNode neighbour in neighbours)
        {
            //neighbour.Cell.TurnOnNeighbourFloor();
        }

        return neighbours;

    }

    public int X { get; set; }
    public int Z { get; set; }
}

public class DepthFirstSearch : MonoBehaviour
{
    // Start is called before the first frame update

    private int numberOfNodesExplored = 0;

    [SerializeField]
    private MazeGenerator mazeGenerator;

    [SerializeField]
    private TMPro.TextMeshProUGUI DepthFirstSearchText;

    public bool isPathShown = false;

    public bool isGeneratingPath = false;

    private IEnumerator GeneratePath()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        numberOfNodesExplored = 0;
        isGeneratingPath = true;
        DepthFirstSearchText.text = "Exploring...";
        List<DepthFirstSearchNode> visited = new List<DepthFirstSearchNode>();
        DepthFirstSearchNode startNode = new DepthFirstSearchNode { Cell = mazeGenerator.mazeGrid[0, 0], X = 0, Z = 0 };
        MazeCell endCell = mazeGenerator.mazeGrid[mazeGenerator.mazeWidth - 1, mazeGenerator.mazeDepth - 1];
        DepthFirstSearchNode endNode = new DepthFirstSearchNode { Cell = endCell, X = mazeGenerator.mazeWidth - 1, Z = mazeGenerator.mazeDepth - 1 };
        Stack<DepthFirstSearchNode> stack = new Stack<DepthFirstSearchNode>();
        stack.Push(startNode);
        while (stack.Count > 0)
        {
            DepthFirstSearchNode currentNode = stack.Pop();

            if (currentNode.Equals(endNode))
            {
                Debug.Log("Found the end node!");
                endNode.PreviousNode = currentNode.PreviousNode;
                break;
            }
            if (!visited.Contains(currentNode))
            {
                visited.Add(currentNode);
                numberOfNodesExplored++;
                List<DepthFirstSearchNode> neighbours = currentNode.GetNeighbours(mazeGenerator.mazeGrid);
                foreach (DepthFirstSearchNode neighbour in neighbours)
                {
                    neighbour.PreviousNode = currentNode;
                    stack.Push(neighbour);
                }
            }
        }
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("DepthFirstSearch generated path, exploring " + numberOfNodesExplored + " nodes! in " + elapsedMs + " ms");
        yield return StartCoroutine(ShowPath(endNode));

    }

    private IEnumerator ShowPath(DepthFirstSearchNode endCell)
    {
        DepthFirstSearchNode currentNode = endCell;

        while (currentNode.PreviousNode != null)
        {
            currentNode.Cell.TurnOnFloor();
            currentNode = currentNode.PreviousNode;
            yield return new WaitForSeconds(0.02f);
        }
        isGeneratingPath = false;
        DepthFirstSearchText.text = "'Y' - hide the path";
    }


    private void UnShowPath()
    {
        foreach (MazeCell cell in mazeGenerator.mazeGrid)
        {
            cell.TurnOffFloor();
            cell.TurnOffNeighbourFloor();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isGeneratingPath)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (mazeGenerator.isMazeGenerated)
            {

                if (isPathShown)
                {
                    UnShowPath();
                    DepthFirstSearchText.text = "'Y' - show the path";
                    isPathShown = false;
                }
                else
                {
                    isPathShown = true;
                    StartCoroutine(GeneratePath());
                }

            }
            else
            {
                Debug.Log("Maze is not generated yet");
            }
        }
    }
}
