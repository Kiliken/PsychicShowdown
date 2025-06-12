using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player player1;
    [SerializeField] Player player2;
    [SerializeField] RoundTimerUI timer;
    [SerializeField] GameObject player1WinText;
    [SerializeField] GameObject player2WinText;
    [SerializeField] GameObject drawText;

    public float timeLimit = 120f;
    //public float timeRemaining;
    public bool gameStarted = false;
    public bool gameEnded = false;

    //TEMPORARY
    private float sceneChangeTime = 2f;
    private float sceneChangeTimer = 0f;
    private bool sceneChanged = false;


    private void Awake()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        //player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
        //player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();
        //timer = GameObject.FindGameObjectWithTag("Timer").GetComponent<RoundTimerUI>();
        //player1WinText = GameObject.FindGameObjectWithTag("P1WinText");
        //player2WinText = GameObject.FindGameObjectWithTag("P2WinText");

        player1.gameManager = this;
        player2.gameManager = this;
        timer.gameManager = this;
        timer.StartTimer(timeLimit);
        gameStarted = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (gameEnded && !sceneChanged)
        {
            if (sceneChangeTimer < sceneChangeTime)
            {
                sceneChangeTimer += Time.deltaTime;
            }
            else
            {
                SceneManager.LoadScene("EndAndRestart");
                sceneChanged = true;
            }
        }
    }


    public void GameOver(int winner)
    {
        if (gameEnded) return;

        // disable inputs for both Players
        player1.SetPlayerActive(false);
        player2.SetPlayerActive(false);

        // pause timer
        timer.timerActive = false;
        timer.gameObject.SetActive(false);

        // add draw
        if (winner == 1)
        {
            // player 1 winner, display text
            //player1WinText.SetActive(true);
            GameData.winner = 1;
        }
        else if (winner == 2)
        {
            // player 2 winner, display text
            //player2WinText.SetActive(true);
            GameData.winner = 2;
        }
        else
        {
            //drawText.SetActive(true);
            GameData.winner = 0;
        }

        gameEnded = true;
    }


    public void CompareHP()
    {
        if (player1.hp > player2.hp)
        {
            GameOver(1);
        }
        else if (player2.hp > player1.hp)
        {
            GameOver(2);
        }
        else
        {
            GameOver(0);
        }
    }
}
