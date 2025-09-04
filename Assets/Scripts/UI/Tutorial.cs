using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialSteps;
    [SerializeField] private TutorialText[] tutorialTexts;
    protected GameSettings gs;
    protected TitleScreen ts;

    [SerializeField] private GameObject firstPage;
    [SerializeField] private GameObject secPage;
    private int curPage = 0;

    [SerializeField] private GameObject controllerIcon;
    public int curTutorialStep = 0;

    [SerializeField] private Image background;
    [SerializeField] private Sprite enBG;
    [SerializeField] private Sprite jpBG;

    // Start is called before the first frame update
    void Start()
    {
        gs = FindObjectOfType<GameSettings>();
        ts = FindObjectOfType<TitleScreen>();
        curPage = 1;
        background.sprite = enBG;
        firstPage.SetActive(true);
        secPage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //TutorialSteps();
        
        if (gs.p1ControllerIsPS)
        {
            if (Input.GetButtonDown("Prev1"))
            {
                Prev();
            }

            if (Input.GetButtonDown("Dash1"))
            {
                Next();
            }

            if (Input.GetButtonDown("AimCancel1"))
            {
                ChangeLanguage();
            }
        }
        else
        {
            if (Input.GetButtonDown("Prev1X"))
            {
                Prev();
            }

            if (Input.GetButtonDown("Dash1X"))
            {
                Next();
            }

            if (Input.GetButtonDown("AimCancel1X"))
            {
                ChangeLanguage();
            }
        
        }
        
    }

    //private void TutorialSteps()
    //{
    //    for (int i = 0; i < tutorialSteps.Length; i++)
    //    {
    //        if (i == curTutorialStep)
    //        {
    //            if (EventSystem.current.currentSelectedGameObject == tutorialSteps[i])
    //            {
    //                curTutorialStep = i;
    //                Debug.Log(curTutorialStep + " selected step");
    //            }
    //        }
    //    }

    //}

    private void Next()
    {
        Debug.Log("Next tutorial page");


        if (curPage == 1)
        {
            firstPage.SetActive(false);
            secPage.SetActive(true);
            curPage = 2;
            EventSystemUpdate.current.SetSelectedGameObject(tutorialSteps[5]);
            curTutorialStep = 5;
        }
        else if (curPage == 2)
        {
            ts.LoadPlayScene();
        }
    }

    private void Prev()
    {
        Debug.Log("Previous tutorial page");
    
        if (curPage == 2)
        {
            secPage.SetActive(false);
            firstPage.SetActive(true);
            curPage = 1;
            EventSystemUpdate.current.SetSelectedGameObject(tutorialSteps[0]);
            curTutorialStep = 0;
        }
        else if (curPage == 1)
        {
            ts.HideTutorialMenu();
            ts.ShowMainMenuPanel();
        }


    }

    private void ChangeLanguage()
    {
        Debug.Log("Change language");
        background.sprite = background.sprite == enBG ? jpBG : enBG;
        foreach (TutorialText tt in tutorialTexts)
        {
            tt.ToggleText();
        }
    }
}
