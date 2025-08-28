using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellHPBar : MonoBehaviour
{
    public Player player;
    public GameObject hpCellPrefab;
    public Transform container;
    public int maxHP = 15;
    public int currentHP = 10;
    [SerializeField] int playerNum;

    private List<GameObject> cells = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        float cellSpacing = hpCellPrefab.GetComponent<RectTransform>().rect.width/120;
        float cellWidth = hpCellPrefab.GetComponent<RectTransform>().rect.width;
        Debug.Log(cellWidth);

        for (int i = 0; i < maxHP; i++)
        {
            GameObject cell = Instantiate(hpCellPrefab, container);
            cells.Add(cell);

            //set anchored position
            RectTransform rt = cell.GetComponent<RectTransform>();
            if (playerNum == 1)
            {
                rt.localPosition = new Vector2(i * (cellWidth + cellSpacing), 0);
            }
            else if (playerNum == 2)
            {
                rt.localPosition = new Vector2(i * (cellWidth + cellSpacing) * -1, 0);
            }
            
        }
        UpdateHPBar();
    }

    public void UpdateHPBar()
    {

        maxHP = player.maxHP;
        currentHP = player.hp;
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetActive(i < currentHP);
        }
    }

}
