using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AStarNode
{
    public MazeCell Cell { get; set; }
    public AStarNode PreviousNode { get; set; } = null;


    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        AStarNode node = (AStarNode)obj;
        return (X == node.X) && (Z == node.Z);

    }

    // override object.GetHashCode
    public override int GetHashCode()
    {

        return base.GetHashCode();
    }

    public List<AStarNode> GetNeighbours(MazeCell[,] mazeGrid)
    {
        List<AStarNode> neighbours = new List<AStarNode>();
        if (X > 0 && mazeGrid[X - 1, Z].rightWall.activeSelf == false)
        {
            neighbours.Add(new AStarNode { Cell = mazeGrid[X - 1, Z], X = X - 1, Z = Z });
        }
        if (X < mazeGrid.GetLength(0) - 1 && mazeGrid[X + 1, Z].leftWall.activeSelf == false)
        {
            neighbours.Add(new AStarNode { Cell = mazeGrid[X + 1, Z], X = X + 1, Z = Z });
        }
        if (Z > 0 && mazeGrid[X, Z - 1].frontWall.activeSelf == false)
        {
            neighbours.Add(new AStarNode { Cell = mazeGrid[X, Z - 1], X = X, Z = Z - 1 });
        }
        if (Z < mazeGrid.GetLength(1) - 1 && mazeGrid[X, Z + 1].backWall.activeSelf == false)
        {
            neighbours.Add(new AStarNode { Cell = mazeGrid[X, Z + 1], X = X, Z = Z + 1 });
        }

        // display neighbours
        foreach (AStarNode neighbour in neighbours)
        {
            //neighbour.Cell.TurnOnNeighbourFloor();
        }

        return neighbours;

    }

    public int G { get; set; } = 0;
    public int H { get; set; } = 0;
    public int F { get; set; } = 0;

    public int X { get; set; }
    public int Z { get; set; }
}

public class AStar : MonoBehaviour
{
    // Start is called before the first frame update

    private int numberOfNodesExplored = 0;

    [SerializeField]
    private MazeGenerator mazeGenerator;

    [SerializeField]
    private TMPro.TextMeshProUGUI AStarText;

    private List<AStarNode> openList = new List<AStarNode>();

    public bool isPathShown = false;

    public bool isGeneratingPath = false;
    private int GetManhattenDistance(AStarNode currentCell, AStarNode neighbour)
    {
        int xDistance = Mathf.Abs(currentCell.X - neighbour.X);
        int zDistance = Mathf.Abs(currentCell.Z - neighbour.Z);
        return xDistance + zDistance;
    }
    void Start()
    {

    }

    private IEnumerator GeneratePath()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        numberOfNodesExplored = 0;
        isGeneratingPath = true;
        List<AStarNode> openList = new List<AStarNode>();
        List<AStarNode> closedList = new List<AStarNode>();
        MazeCell startCell = mazeGenerator.mazeGrid[0, 0];

        MazeCell endCell = mazeGenerator.mazeGrid[mazeGenerator.mazeWidth - 1, mazeGenerator.mazeDepth - 1];
        AStarNode endNode = new AStarNode { Cell = endCell, X = mazeGenerator.mazeWidth - 1, Z = mazeGenerator.mazeDepth - 1 };
        openList.Add(new AStarNode { Cell = startCell, X = 0, Z = 0 });
        AStarNode currentNode = null;
        while (openList.Count > 0)
        {
            currentNode = openList[0];
            //currentNode.Cell.TurnOnFloor();
            numberOfNodesExplored++;
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F || openList[i].F == currentNode.F && openList[i].H < currentNode.H)
                {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            if (currentNode.Cell == endCell)
            {
                break;
            }
            foreach (AStarNode neighbour in currentNode.GetNeighbours(mazeGenerator.mazeGrid))
            {
                if (neighbour == null || closedList.Contains(neighbour))
                {
                    continue;
                }

                int moveCost = currentNode.G + GetManhattenDistance(currentNode, neighbour);
                if (moveCost < neighbour.G || !openList.Contains(neighbour))
                {
                    neighbour.G = moveCost;
                    neighbour.H = GetManhattenDistance(neighbour, endNode);
                    neighbour.F = neighbour.G + neighbour.H;
                    neighbour.PreviousNode = currentNode;
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
            //yield return new WaitForSeconds(0.5f);
        }
        watch.Stop();
        Debug.Log("A-Star generated path, exploring " + numberOfNodesExplored + " nodes!" + " in " + watch.ElapsedMilliseconds + " ms");
        yield return new WaitForSeconds(0.02f);
        StartCoroutine(ShowPath(currentNode));
    }

    private IEnumerator ShowPath(AStarNode endCell)
    {
        AStarNode currentNode = endCell;
        Debug.Log("A-Star generated path, exploring " + numberOfNodesExplored + " nodes!");
        while (currentNode.PreviousNode != null)
        {
            currentNode.Cell.TurnOnFloor();
            currentNode = currentNode.PreviousNode;
            yield return new WaitForSeconds(0.02f);
        }
        isGeneratingPath = false;
        AStarText.text = "'R' - hide the path";
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (mazeGenerator.isMazeGenerated)
            {

                if (isPathShown)
                {
                    UnShowPath();
                    AStarText.text = "'R' - show the path";
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
