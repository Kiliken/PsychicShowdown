using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ControllerIcon : MonoBehaviour
{

    [SerializeField] private Tutorial tutorial;
    [SerializeField] private Image targetImage;      // assign your UI Image here
    [SerializeField] private Sprite[] sprites;       // assign all sprites in Inspector



        // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetSpriteByIndex(tutorial.curTutorialStep);
        //Debug.Log("Current tutorial step: " + tutorial.curTutorialStep);
    }

    public void SetSpriteByIndex(int i)
    {
        if (sprites != null && sprites.Length > i && i >= 0)
        {
            targetImage.sprite = sprites[i];
        }
        else
        {
            Debug.LogWarning("Invalid sprite index: " + i);
        }
    }
}
