using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.UI;
using UnityEngine.EventSystems;
using System.Transactions;

public class TitleScreen : MonoBehaviour
{
    string sceneToUse = string.Empty;

    [SerializeField] Transform worldObj;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    //private GameObject curDefaultButton;
    private GameObject lastValidSelection;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject firstSettingsButton;
    private float navCooldown = 0.2f;
    private float lastNavTime = 0f;

    private float volume = 0.5f;
    private float p1Sensitivity = 0.5f;
    private float p2Sensitivity = 0.5f;
    private bool p1controlisPS = true;
    private bool p2controlisPS = true;

    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider p1SensitivitySlider;
    [SerializeField] private Slider p2SensitivitySlider;

    [SerializeField] private GameSettings gameSettings;


    // Start is called before the first frame update
    void Start()
    {
        sceneToUse = FindAnyObjectByType<DebugController>().sceneName != string.Empty ? FindAnyObjectByType<DebugController>().sceneName : "BetaPortFHD";
        Debug.Log(sceneToUse);
        ShowMainMenuPanel();
        //EventSystem.current.SetSelectedGameObject(firstButton);
        //mainMenuPanel.SetActive(true);
        //curDefaultButton = firstButton;
        //settingsPanel.SetActive(false);
        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        SetSoundVolume(soundSlider.value);
        p1SensitivitySlider.onValueChanged.AddListener(SetP1Sensitivity);
        SetP1Sensitivity(p1SensitivitySlider.value);
        p2SensitivitySlider.onValueChanged.AddListener(SetP2Sensitivity);
        SetP2Sensitivity(p2SensitivitySlider.value);

    }


    // Update is called once per frame
    private void Update()
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

        
        //if (EventSystem.current.currentSelectedGameObject == null) 
        //{
        //    EventSystem.current.SetSelectedGameObject(curDefaultButton);
        //    Debug.Log("Current selected: " + EventSystem.current.currentSelectedGameObject?.name);
        //}
        
    }

    private void PlayerInput()
    {
        float vertical1 = Input.GetAxisRaw("Vertical1");
        float vertical2 = Input.GetAxisRaw("Vertical2");

        float now = Time.time;
        if (now - lastNavTime > navCooldown)
        {
            if (vertical1 > 0.5f || vertical2 > 0.5f)
            {
                Navigate(Vector2.up);
                lastNavTime = now;
            }
            else if (vertical1 < -0.5f || vertical2 < -0.5f)
            {
                Navigate(Vector2.down);
                lastNavTime = now;
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

    void FixedUpdate()
    {
        worldObj.Rotate(0, 5 * Time.deltaTime, 0);
    }

    public void LoadPlayScene(){
        Debug.Log("Settings values: \nvolume:" + volume + "\np1 sensitivity: " + p1Sensitivity + "\np1 Controller:" + (p1controlisPS ? "PlayStation" : "Xbox"
            + "\np2 sensitivity: " + p2Sensitivity + "\np2 Controller: " + (p2controlisPS ? "PlayStation" : "Xbox")));
        gameSettings.SetSettings(volume, p1Sensitivity, p1controlisPS, p2Sensitivity, p2controlisPS);
        Debug.Log("Loading scene: " + sceneToUse);
        SceneManager.LoadScene(sceneToUse);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowSettingPanel()
    {
        //Debug.Log("Showing settings panel");
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSettingsButton);
        //curDefaultButton = firstSettingsButton;
        lastValidSelection = firstSettingsButton;
    }

    public void ShowMainMenuPanel()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstButton);
        //curDefaultButton = firstButton;\
        lastValidSelection = firstButton;
    }

    public void SetSoundVolume(float value)
    {
        volume = value;
        Debug.Log("Sound volume set to: " + volume);

    }

    public void SetControl(int p, bool isPS)
    {

        if (p == 1)
        {
            SetP1ControlType(isPS);
        }
        else if (p == 2)
        {
            SetP2ControlType(isPS);
        }
    }
    public void SetP1Sensitivity(float value)
    {
        p1Sensitivity = value;
        Debug.Log("Player 1 sensitivity set to: " + p1Sensitivity);
    }

    public void SetP2Sensitivity(float value)
    {
        p2Sensitivity = value;
        Debug.Log("Player 2 sensitivity set to: " + p2Sensitivity);
    }

    public void SetP1ControlType(bool isPS)
    {
        p1controlisPS = isPS;
        Debug.Log("Player 1 control type set to: " + (p1controlisPS ? "PlayStation" : "Xbox"));
    }

    public void SetP2ControlType(bool isPS)
    {
        p2controlisPS = isPS;
        Debug.Log("Player 2 control type set to: " + (p2controlisPS ? "PlayStation" : "Xbox"));
    }
}
