using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    public GameObject playerObject;
    private PlayerMovement playerMovement;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        player = playerObject.GetComponent<Player>();
        GetComponentInChildren<JumpDotsUI>().player = playerMovement;
        GetComponentInChildren<DashDotsUI>().player = playerMovement;
        GetComponentInChildren<CellHPBar>().player = player;
        GetComponentInChildren<CellHPBar>().UpdateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
