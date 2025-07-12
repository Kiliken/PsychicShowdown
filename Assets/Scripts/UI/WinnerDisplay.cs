using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinnerDisplay : MonoBehaviour
{
    // Start is called before the first frame update

    TextMeshProUGUI winnerText;
    [SerializeField] Transform winner;
    [SerializeField] Transform loser;
    [SerializeField] Transform torii;


    void Start()
    {
        //GameData.winner = 1;
        winnerText = GetComponent<TextMeshProUGUI>();

        if (GameData.winner == 1)
        {
            winnerText.text = "プレイヤー１勝";
            winner.GetChild(0).gameObject.SetActive(true);
            loser.GetChild(1).gameObject.SetActive(true);
            torii.GetChild(0).gameObject.SetActive(true);
        } 
        else if (GameData.winner == 2)
        {
            winnerText.text = "プレイヤー２勝";
            winner.GetChild(1).gameObject.SetActive(true);
            loser.GetChild(0).gameObject.SetActive(true);
            torii.GetChild(1).gameObject.SetActive(true);

        }
        else if (GameData.winner == 0)
        {
            winnerText.text = "引き分け";
        }
    }

    public void ToTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    
}
