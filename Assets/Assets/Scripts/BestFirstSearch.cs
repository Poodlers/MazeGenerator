using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestFirstSearchNode
{
    public MazeCell Cell { get; set; }
    public BestFirstSearchNode PreviousNode { get; set; } = null;


    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        BestFirstSearchNode node = (BestFirstSearchNode)obj;
        return (X == node.X) && (Z == node.Z);

    }

    // override object.GetHashCode
    public override int GetHashCode()
    {

        return base.GetHashCode();
    }

    public List<BestFirstSearchNode> GetNeighbours(MazeCell[,] mazeGrid)
    {
        List<BestFirstSearchNode> neighbours = new List<BestFirstSearchNode>();
        if (X > 0 && mazeGrid[X - 1, Z].rightWall.activeSelf == false)
        {
            neighbours.Add(new BestFirstSearchNode { Cell = mazeGrid[X - 1, Z], X = X - 1, Z = Z });
        }
        if (X < mazeGrid.GetLength(0) - 1 && mazeGrid[X + 1, Z].leftWall.activeSelf == false)
        {
            neighbours.Add(new BestFirstSearchNode { Cell = mazeGrid[X + 1, Z], X = X + 1, Z = Z });
        }
        if (Z > 0 && mazeGrid[X, Z - 1].frontWall.activeSelf == false)
        {
            neighbours.Add(new BestFirstSearchNode { Cell = mazeGrid[X, Z - 1], X = X, Z = Z - 1 });
        }
        if (Z < mazeGrid.GetLength(1) - 1 && mazeGrid[X, Z + 1].backWall.activeSelf == false)
        {
            neighbours.Add(new BestFirstSearchNode { Cell = mazeGrid[X, Z + 1], X = X, Z = Z + 1 });
        }

        // display neighbours
        foreach (BestFirstSearchNode neighbour in neighbours)
        {
            //neighbour.Cell.TurnOnNeighbourFloor();
        }

        return neighbours;

    }

    public int H { get; set; } = 0;

    public int X { get; set; }
    public int Z { get; set; }
}

public class BestFirstSearch : MonoBehaviour
{
    // Start is called before the first frame update

    private int numberOfNodesExplored = 0;

    [SerializeField]
    private MazeGenerator mazeGenerator;

    [SerializeField]
    private TMPro.TextMeshProUGUI BestFirstSearchText;

    private List<BestFirstSearchNode> openList = new List<BestFirstSearchNode>();

    public bool isPathShown = false;

    public bool isGeneratingPath = false;
    private int GetManhattenDistance(BestFirstSearchNode currentCell, BestFirstSearchNode neighbour)
    {
        int xDistance = Mathf.Abs(currentCell.X - neighbour.X);
        int zDistance = Mathf.Abs(currentCell.Z - neighbour.Z);
        return xDistance + zDistance;
    }

    private IEnumerator GeneratePath()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        numberOfNodesExplored = 0;
        isGeneratingPath = true;
        List<BestFirstSearchNode> openList = new List<BestFirstSearchNode>();
        List<BestFirstSearchNode> closedList = new List<BestFirstSearchNode>();
        MazeCell startCell = mazeGenerator.mazeGrid[0, 0];

        MazeCell endCell = mazeGenerator.mazeGrid[mazeGenerator.mazeWidth - 1, mazeGenerator.mazeDepth - 1];
        BestFirstSearchNode endNode = new BestFirstSearchNode { Cell = endCell, X = mazeGenerator.mazeWidth - 1, Z = mazeGenerator.mazeDepth - 1 };
        openList.Add(new BestFirstSearchNode { Cell = startCell, X = 0, Z = 0 });
        BestFirstSearchNode currentNode = null;
        while (openList.Count > 0)
        {
            currentNode = openList[0];
            //currentNode.Cell.TurnOnFloor();
            numberOfNodesExplored++;
            for (int i = 1; i < openList.Count; i++)
            {
                //always explore the node with the lowest H value, the node which is closer to end Node
                if (openList[i].H < currentNode.H)
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
            foreach (BestFirstSearchNode neighbour in currentNode.GetNeighbours(mazeGenerator.mazeGrid))
            {
                if (neighbour == null || closedList.Contains(neighbour))
                {
                    continue;
                }


                if (!openList.Contains(neighbour))
                {

                    neighbour.H = GetManhattenDistance(neighbour, endNode);

                    neighbour.PreviousNode = currentNode;
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }

        }
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("BestFirstSearch generated path, exploring " + numberOfNodesExplored + " nodes! in " + elapsedMs + " ms");
        yield return new WaitForSeconds(0.02f);
        StartCoroutine(ShowPath(currentNode));
    }

    private IEnumerator ShowPath(BestFirstSearchNode endCell)
    {
        BestFirstSearchNode currentNode = endCell;
        while (currentNode.PreviousNode != null)
        {
            currentNode.Cell.TurnOnFloor();
            currentNode = currentNode.PreviousNode;
            yield return new WaitForSeconds(0.02f);
        }
        isGeneratingPath = false;
        BestFirstSearchText.text = "'T' - hide the path";
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
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (mazeGenerator.isMazeGenerated)
            {

                if (isPathShown)
                {
                    UnShowPath();
                    BestFirstSearchText.text = "'T' - show the path";
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
