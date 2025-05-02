using UnityEngine;
using UnityEngine.UI;

public class RoundTimerUI : MonoBehaviour
{
    public float roundDuration = 120f; // 2 minutes
    private float timer;

    public Text timerText; // Legacy UI Text component

    void Start()
    {
        timer = roundDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0f);

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
