using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreadthFirstSearchNode
{
    public MazeCell Cell { get; set; }
    public BreadthFirstSearchNode PreviousNode { get; set; } = null;

    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        BreadthFirstSearchNode node = (BreadthFirstSearchNode)obj;
        return (X == node.X) && (Z == node.Z);

    }

    // override object.GetHashCode
    public override int GetHashCode()
    {

        return base.GetHashCode();
    }

    public List<BreadthFirstSearchNode> GetNeighbours(MazeCell[,] mazeGrid)
    {
        List<BreadthFirstSearchNode> neighbours = new List<BreadthFirstSearchNode>();
        if (X > 0 && mazeGrid[X - 1, Z].rightWall.activeSelf == false)
        {
            neighbours.Add(new BreadthFirstSearchNode { Cell = mazeGrid[X - 1, Z], X = X - 1, Z = Z });
        }
        if (X < mazeGrid.GetLength(0) - 1 && mazeGrid[X + 1, Z].leftWall.activeSelf == false)
        {
            neighbours.Add(new BreadthFirstSearchNode { Cell = mazeGrid[X + 1, Z], X = X + 1, Z = Z });
        }
        if (Z > 0 && mazeGrid[X, Z - 1].frontWall.activeSelf == false)
        {
            neighbours.Add(new BreadthFirstSearchNode { Cell = mazeGrid[X, Z - 1], X = X, Z = Z - 1 });
        }
        if (Z < mazeGrid.GetLength(1) - 1 && mazeGrid[X, Z + 1].backWall.activeSelf == false)
        {
            neighbours.Add(new BreadthFirstSearchNode { Cell = mazeGrid[X, Z + 1], X = X, Z = Z + 1 });
        }

        // display neighbours
        foreach (BreadthFirstSearchNode neighbour in neighbours)
        {
            //neighbour.Cell.TurnOnNeighbourFloor();
        }

        return neighbours;

    }

    public int X { get; set; }
    public int Z { get; set; }
}

public class BreadthFirstSearchManager : MonoBehaviour
{
    // Start is called before the first frame update

    private int numberOfNodesExplored = 0;

    [SerializeField]
    private MazeGenerator mazeGenerator;

    [SerializeField]
    private TMPro.TextMeshProUGUI BreadthFirstSearchText;

    public bool isPathShown = false;

    public bool isGeneratingPath = false;

    private IEnumerator GeneratePath()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        numberOfNodesExplored = 0;
        isGeneratingPath = true;
        BreadthFirstSearchText.text = "Exploring...";
        BreadthFirstSearchNode startCell = new BreadthFirstSearchNode { Cell = mazeGenerator.mazeGrid[0, 0], X = 0, Z = 0 };
        BreadthFirstSearchNode endCell = new BreadthFirstSearchNode { Cell = mazeGenerator.mazeGrid[mazeGenerator.mazeWidth - 1, mazeGenerator.mazeDepth - 1], X = mazeGenerator.mazeWidth - 1, Z = mazeGenerator.mazeDepth - 1 };
        Queue<BreadthFirstSearchNode> queue = new Queue<BreadthFirstSearchNode>();
        List<BreadthFirstSearchNode> visited = new List<BreadthFirstSearchNode>();
        queue.Enqueue(startCell);
        while (queue.Count > 0)
        {
            BreadthFirstSearchNode currentNode = queue.Dequeue();
            visited.Add(currentNode);
            if (currentNode.Equals(endCell))
            {
                endCell.PreviousNode = currentNode.PreviousNode;
                break;
            }
            List<BreadthFirstSearchNode> neighbours = currentNode.GetNeighbours(mazeGenerator.mazeGrid);
            foreach (BreadthFirstSearchNode neighbour in neighbours)
            {
                if (!visited.Contains(neighbour))
                {
                    neighbour.PreviousNode = currentNode;
                    queue.Enqueue(neighbour);
                }
            }
            numberOfNodesExplored++;


        }
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("BreadthFirstSearch generated path, exploring " + numberOfNodesExplored + " nodes! in " + elapsedMs + " ms");
        yield return StartCoroutine(ShowPath(endCell));


    }

    private IEnumerator ShowPath(BreadthFirstSearchNode endCell)
    {
        BreadthFirstSearchNode currentNode = endCell;

        while (currentNode.PreviousNode != null)
        {
            currentNode.Cell.TurnOnFloor();
            currentNode = currentNode.PreviousNode;
            yield return new WaitForSeconds(0.02f);
        }
        isGeneratingPath = false;
        BreadthFirstSearchText.text = "'U' - hide the path";
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
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (mazeGenerator.isMazeGenerated)
            {

                if (isPathShown)
                {
                    UnShowPath();
                    BreadthFirstSearchText.text = "'U' - show the path";
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
