using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinnerDisplay : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI winnerText;
    void Start()
    {
        //GameData.winner = 1;
        winnerText = GetComponent<TextMeshProUGUI>();

        if (GameData.winner == 1)
        {
            winnerText.text = "プレイヤー１勝";
        } 
        else if (GameData.winner == 2)
        {
            winnerText.text = "プレイヤー２勝";
        }
        else if (GameData.winner == 0)
        {
            winnerText.text = "引き分け";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
