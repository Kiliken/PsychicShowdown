using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    public GameObject playerObject;
    private PlayerMovement playerMovement;
    private Player player;
    [SerializeField] private bool isP1;
    // Start is called before the first frame update

    void Awake()
    {
        if (isP1)
        {
            playerObject = GameObject.Find("Player1");
        }
        else
        {
            playerObject = GameObject.Find("Player2");
        }
        
        //playerObject.player.
    }

    void Start()
    {
        
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        player = playerObject.GetComponent<Player>();
        player.hpBar = GetComponentInChildren<CellHPBar>();
        GetComponentInChildren<JumpDotsUI>().player = playerMovement;
        GetComponentInChildren<DashDotsUI>().player = playerMovement;
        GetComponentInChildren<CellHPBar>().player = player;
        GetComponentInChildren<CellHPBar>().UpdateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHPBar()
    {

    }
}
