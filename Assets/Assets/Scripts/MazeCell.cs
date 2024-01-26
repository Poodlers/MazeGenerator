using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    public GameObject leftWall;

    [SerializeField]
    public GameObject rightWall;

    [SerializeField]
    public GameObject frontWall;

    [SerializeField]

    public GameObject backWall;

    [SerializeField]
    private GameObject visitedByAlgorithm;

    [SerializeField]
    private GameObject neighbourCellAlgorithm;

    [SerializeField]
    private GameObject unvisitedBlock;

    public bool IsVisited { get; set; } = false;

    public void Visit()
    {
        IsVisited = true;
        unvisitedBlock.SetActive(false);
    }

    public void TurnOffFloor()
    {
        visitedByAlgorithm.SetActive(false);
    }

    public void TurnOnFloor()
    {
        visitedByAlgorithm.SetActive(true);
    }

    public void TurnOnNeighbourFloor()
    {
        neighbourCellAlgorithm.SetActive(true);
    }

    public void TurnOffNeighbourFloor()
    {
        neighbourCellAlgorithm.SetActive(false);
    }

    public void ClearLeftWall()
    {
        leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        backWall.SetActive(false);
    }
}
