using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public float soundVolume;
    public bool p1ControllerIsPS;
    public float p1Sensitivity;
    public bool p2ControllerIsPS;
    public float p2Sensitivity;


    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameSettings");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        soundVolume = 0.5f; // Default volume
        p1Sensitivity = 0.5f; // Default player 1 sensitivity
        p2Sensitivity = 0.5f; // Default player 2 sensitivity
        p1ControllerIsPS = false; 
        p2ControllerIsPS = false; // Default controller type for player 2
        Cursor.visible = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSettings(float vol, float p1sen, bool p1con, float p2sen, bool p2con)
    {
        soundVolume = vol;
        p1Sensitivity = p1sen;
        p2Sensitivity = p2sen;
        p1ControllerIsPS = p1con;
        p2ControllerIsPS = p2con;
    }
}
