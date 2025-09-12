using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

    [SerializeField] private NetController netController;

    private GameObject lastValidSelection;
    private float navCooldown = 0.2f;
    private float lastNavTime = 0f;
    private void Awake()
    {
        p1PauseMenu = GameObject.Find("P1PauseMenu");
        //p2PauseMenu = GameObject.Find("P2PauseMenu");
    }

    // Start is called before the first frame update
    void Start()
    {

        p1PauseMenu.SetActive(false);
        //p2PauseMenu.SetActive(false);
        //GameObject p1ES = GameObject.Find("EventSystemP1");
        //GameObject p2ES = GameObject.Find("EventSystemP2");
        //p1EventSystem = p1ES.GetComponent<EventSystemUpdate>();
        //p2EventSystem = p2ES.GetComponent<EventSystemUpdate>();
    }

    // Update is called once per frame
    void Update()
    {
        var current = EventSystem.current.currentSelectedGameObject;

        if (current != null && current != lastValidSelection)
        {

            lastValidSelection = current;
        }


        PlayerInput();


        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastValidSelection);
        }
    }

    private void PlayerInput()
    {
        float vertical1 = Input.GetAxisRaw("Vertical1");

        float vertical2 = Input.GetAxisRaw("Vertical2");

        float vertical3 = Input.GetAxisRaw("Vertical3");

        float now = Time.time;
        if (now - lastNavTime > navCooldown)
        {
            if (vertical1 > 0.5f || vertical2 > 0.5f || vertical3 > 0.5f)
            {
                Navigate(Vector2.up);
                lastNavTime = now;
            }
            else if (vertical1 < -0.5f || vertical2 < -0.5f || vertical3 < -0.5f)
            {
                Navigate(Vector2.down);
                lastNavTime = now;
            }

            if (vertical3 != 0f)
            {
                Debug.Log("Third controller input detected: " + vertical3);
            }
        }
    }

    private void Navigate(Vector2 dir)
    {
        var cur = EventSystem.current.currentSelectedGameObject;
        if (cur == null) return;

        Selectable selectable = cur.GetComponent<Selectable>();
        if (selectable == null) return;

        Selectable next = null;
        if (dir == Vector2.up) next = selectable.FindSelectableOnUp();
        if (dir == Vector2.down) next = selectable.FindSelectableOnDown();
        if (dir == Vector2.left) next = selectable.FindSelectableOnLeft();
        if (dir == Vector2.right) next = selectable.FindSelectableOnRight();

        if (next != null)
        {
            EventSystem.current.SetSelectedGameObject(next.gameObject);
        }

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
        netController.DestroyNetThread();
        SceneManager.LoadScene("OnlineTitleScreen");
    }
}