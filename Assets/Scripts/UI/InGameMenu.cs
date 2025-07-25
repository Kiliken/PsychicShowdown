using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    private bool isP1MenuActive = false;
    private bool isP2MenuActive = false;

    [SerializeField] private GameObject p1PauseMenu;
    [SerializeField] private GameObject p2PauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        p1PauseMenu.SetActive(false);
        p2PauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowPauseMenu(int playerNo)
    {
        if (playerNo == 1)
        {
            Debug.Log("Showing pause menu for Player 1");
            p1PauseMenu.SetActive(true);
            isP1MenuActive = true;
        }
        else if (playerNo == 2)
        {
            Debug.Log("Showing pause menu for Player 2");
            p2PauseMenu.SetActive(true);
            isP2MenuActive = true;
        }

    }

    public void HidePauseMenu(int playerNo)
    {
        if (playerNo == 1)
        {
            Debug.Log("Hiding pause menu for Player 1");
            p1PauseMenu.SetActive(false);
            isP1MenuActive = false;
        }
        else if (playerNo == 2)
        {
            Debug.Log("Hiding pause menu for Player 2");
            p2PauseMenu.SetActive(false);
            isP2MenuActive = false;
        }

    }

    public bool isPlayerPauseMenuActive(int playerNo)
    {
        if (playerNo == 1)
        {
            return isP1MenuActive;
        }
        else if (playerNo == 2)
        {
            return isP2MenuActive;
        }
        return false;
    }
}