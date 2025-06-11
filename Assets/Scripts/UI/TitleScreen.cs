using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    string sceneToUse = string.Empty;

    [SerializeField] Transform worldObj;
    // Start is called before the first frame update
    void Start()
    {
        sceneToUse = FindAnyObjectByType<DebugController>().sceneName != string.Empty ? FindAnyObjectByType<DebugController>().sceneName : "AlphaPortFHD";
        Debug.Log(sceneToUse);
    }

    // Update is called once per frame
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
}
