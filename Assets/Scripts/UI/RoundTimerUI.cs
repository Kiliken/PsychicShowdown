using UnityEngine;
using UnityEngine.UI;

public class RoundTimerUI : MonoBehaviour
{
    public GameManager gameManager;
    public float roundDuration = 120f; // 2 minutes
    public float timer;
    private bool timeOut = false;
    public bool timerActive = false;


    public Text timerText; // Legacy UI Text component

    void Start()
    {
        //timer = roundDuration;
    }

    void Update()
    {
        if (!timerActive || timeOut) return;

        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0f);

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);

        if(timer <= 0)
        {
            gameManager.CompareHP();
            timeOut = true;
        }
    }

    public void StartTimer(float time)
    {
        timer = time;
        timerActive = true;
    }
}
