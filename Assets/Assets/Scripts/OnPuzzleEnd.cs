using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPuzzleEnd : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private Movement player;
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Puzzle Complete");
            StartCoroutine(gameManager.EndLevel());
            player.canMove = false;
            //SceneManager.LoadScene("Maze");
        }
    }
}
