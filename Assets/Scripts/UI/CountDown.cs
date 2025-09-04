using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownText;
    private float countDownTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        countDownText.text = "3";
    }

    // Update is called once per frame
    void Update()
    {
        countDownTime = countDownTime - Time.deltaTime;
        countDownText .text = Mathf.Ceil(countDownTime).ToString();
        if (countDownTime <= 0)
        {
            countDownText.text = "GO!";
            StartCoroutine(WaitAndDisable());
        }

    }

    private IEnumerator WaitAndDisable()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
