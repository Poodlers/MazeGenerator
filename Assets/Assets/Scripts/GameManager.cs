using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private TextMeshProUGUI timeText;
    [SerializeField]
    private TextMeshProUGUI solveTheMazeText;
    [SerializeField]
    private TextMeshProUGUI endLevelText;

    [SerializeField]
    private TextMeshProUGUI loadingText;
    private int time = 0;

    private bool isEnd = false;
    public void Start()
    {
        timeText.text = "Time: " + time;
        StartCoroutine(CountTime());
    }

    IEnumerator CountTime()
    {
        while (!isEnd)
        {
            timeCount();
            timeText.text = "Time: " + time;
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator EndLevel()
    {
        StopCounting();
        solveTheMazeText.gameObject.SetActive(false);
        endLevelText.text = "You finished the level in " + time + " seconds! \n Congratulations! \n Restarting in 3 seconds";
        endLevelText.gameObject.SetActive(true);
        loadingText.text = "Loading next level";
        loadingText.gameObject.SetActive(true);
        var count = 0;
        while (count < 3)
        {
            loadingText.text += ".";
            yield return new WaitForSeconds(3f);
            count++;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    void timeCount()
    {
        time += 1;
    }

    public void StopCounting()
    {
        isEnd = true;
        StopCoroutine(CountTime());
    }

}
