using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    private bool isP1MenuActive = false;
    private bool isP2MenuActive = false;

    [SerializeField] private GameObject p1PauseMenu;
    [SerializeField] private GameObject p2PauseMenu;

    [SerializeField] private EventSystemUpdate p1EventSystem;
    [SerializeField] private EventSystemUpdate p2EventSystem;

    [SerializeField] private GameObject p1firstSelected;
    [SerializeField] private GameObject p2firstSelected;

    private void Awake()
    {
        p1PauseMenu = GameObject.Find("P1PauseMenu");
        p2PauseMenu = GameObject.Find("P2PauseMenu");
    }

    // Start is called before the first frame update
    void Start()
    {

        p1PauseMenu.SetActive(false);
        p2PauseMenu.SetActive(false);
        GameObject p1ES = GameObject.Find("EventSystemP1");
        GameObject p2ES = GameObject.Find("EventSystemP2");
        p1EventSystem = p1ES.GetComponent<EventSystemUpdate>();
        p2EventSystem = p2ES.GetComponent<EventSystemUpdate>();
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
            EventSystemUpdate eventSystem = p1EventSystem;
            eventSystem.SetSelectedGameObject(null); // clear selection
            eventSystem.SetSelectedGameObject(p1firstSelected);
        }
        else if (playerNo == 2)
        {
            Debug.Log("Showing pause menu for Player 2");
            p2PauseMenu.SetActive(true);
            isP2MenuActive = true;
            EventSystemUpdate eventSystem = p2EventSystem;
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(p2firstSelected);
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


    public void QuitToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");
        SceneManager.LoadScene("TitleScreen");
    }
}