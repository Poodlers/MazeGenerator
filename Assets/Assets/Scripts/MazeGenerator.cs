using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{

    private Camera mainCamera;

    [SerializeField]
    private GameObject mazeCellPrefab;

    [SerializeField]
    private GameObject gameEndObject;

    [SerializeField]
    public int mazeWidth = 10;

    [SerializeField]
    public int mazeDepth = 10;

    public MazeCell[,] mazeGrid;

    public bool isMazeGenerated = false;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(mazeWidth / 2f - 0.5f, mazeWidth * 1.1f, mazeDepth / 1.7f + 0.6f);
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                GameObject mazeCell = Instantiate(mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
                mazeCell.transform.parent = transform;
                mazeGrid[x, z] = mazeCell.GetComponent<MazeCell>();
            }
        }

        StartCoroutine(
                GenerateMaze());
    }

    private IEnumerator GenerateMaze()
    {
        yield return StartCoroutine(GenerateMaze(null, mazeGrid[0, 0]));
        mazeGrid[0, 0].ClearLeftWall();
        mazeGrid[mazeWidth - 1, mazeDepth - 1].ClearRightWall();
        gameEndObject.transform.position = new Vector3(mazeWidth - 1, 0.5f, mazeDepth - 1);
        isMazeGenerated = true;
    }

    private IEnumerator GenerateMaze(MazeCell previouCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previouCell, currentCell);
        MazeCell nextCell;
        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);
            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);

    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(c => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x > 0 && !mazeGrid[x - 1, z].IsVisited)
        {
            yield return mazeGrid[x - 1, z];
        }
        if (x + 1 < mazeWidth && !mazeGrid[x + 1, z].IsVisited)
        {
            yield return mazeGrid[x + 1, z];
        }
        if (z > 0 && !mazeGrid[x, z - 1].IsVisited)
        {
            yield return mazeGrid[x, z - 1];
        }
        if (z + 1 < mazeDepth && !mazeGrid[x, z + 1].IsVisited)
        {
            yield return mazeGrid[x, z + 1];
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null) return;

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
