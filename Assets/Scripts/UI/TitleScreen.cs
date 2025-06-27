using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.UI;
using UnityEngine.Rendering.UI;
using UnityEngine.EventSystems;
using System.Transactions;
using UnityEditor.UI;

public class TitleScreen : MonoBehaviour
{
    string sceneToUse = string.Empty;

    [SerializeField] Transform worldObj;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject firstButton;

    // Start is called before the first frame update
    void Start()
    {
        sceneToUse = FindAnyObjectByType<DebugController>().sceneName != string.Empty ? FindAnyObjectByType<DebugController>().sceneName : "AlphaPortFHD";
        Debug.Log(sceneToUse);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }


    // Update is called once per frame
    private void Update()
    {

        PlayerInput();
    }

    private void PlayerInput()
    {
        float horizontal1 = Input.GetAxisRaw("Horizontal1");

        float vertical1 = Input.GetAxisRaw("Vertical1");
        float horizontal2 = Input.GetAxisRaw("Horizontal2");

        float vertical2 = Input.GetAxisRaw("Vertical2");
        if (vertical1 > 0f || vertical2 > 0f)
        {
            Navigate(Vector2.up);
        }
        else if (vertical1 < 0f || vertical2 < 0f)
        {
            Navigate(Vector2.down);
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

    void FixedUpdate()
    {
        worldObj.Rotate(0, 5 * Time.deltaTime, 0);
    }

    public void LoadPlayScene(){
        SceneManager.LoadScene(sceneToUse);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowSettingPanel()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ShowMainMenuPanel()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
