using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMenuControl : MonoBehaviour
{

    public string pauseBtn = "Pause1X";
    public string yInput = "Vertical1";
    public string confirmBtn = "Jump1X";

    public bool isMenuActive = false;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerMovement movementScript;
    [SerializeField] private CameraController camController;
    private float lastNavTime = 0f; // Track last navigation time

    // Start is called before the first frame update
    void Start()
    {
        HidePauseMenu();
    }

    // Update is called once per frame
    void Update()
    {
        MenuInput();
    }

    //Activates and runs the in game menu.
    public void InGameMenuInput()
    {
        if (Input.GetButtonDown(pauseBtn))
        {

            Debug.Log("Pause button pressed");
            if (isMenuActive)
            {
                HidePauseMenu();
            }
            else
            {
                ShowPauseMenu();
            }

        }

        if (isMenuActive)
        {
            // If the pause menu is active, disable player movement and camera control
            movementScript.inputActive = false;
            camController.inputActive = false;
            //Debug.Log("set inactive" + playerNo);
        }
        else
        {
            // If the pause menu is not active, enable player movement and camera control
            movementScript.inputActive = true;
            camController.inputActive = true;
            //Debug.Log("set active" + playerNo);
        }
    }

    // Continue the game from the pause menu by enabling player movement and camera control
    public void ContinueGame()
    {
        HidePauseMenu();
        movementScript.inputActive = true;
        camController.inputActive = true;
    }

    private void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        isMenuActive = true;
        movementScript.inputActive = false;
        camController.inputActive = false;
    }

    private void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
        isMenuActive = false;
        movementScript.inputActive = true;
        camController.inputActive = true;
    }




    // Handle menu navigation input
    private void MenuInput()
    {
        float vertical;

        vertical = Input.GetAxis(yInput);

        float now = Time.time;
        float navCooldown = 0.2f; // Cooldown time in seconds

        if (now - lastNavTime > navCooldown)
        {
            if (vertical > 0.5f)
            {
                Navigate(Vector2.up);
                lastNavTime = now;
            }
            else if (vertical < -0.5f)
            {
                Navigate(Vector2.down);
                lastNavTime = now;
            }

        }
    }

    //Navigate through UI elements using joystick
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

    public void QuitToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");
        SceneManager.LoadScene("TitleScreen");
    }
}
