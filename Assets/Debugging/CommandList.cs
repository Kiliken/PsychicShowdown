using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommandList {
    //Write here your command
    public static DebugCommand<string> LOG {get;} = new DebugCommand<string>("log", "logs", "log", (x) =>{
            Debug.Log(x);
        });

    public static DebugCommand<string> LOADSCENE { get; } = new DebugCommand<string>("loadScene", "Loads a scene", "loadScene", (x) => {
        SceneManager.LoadScene(x);
    });

    public static DebugCommand<string> RELOAD { get; } = new DebugCommand<string>("reload", "Reload current scene", "reload", (x) => {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    });

    public static DebugCommand<string> CONTROLLERTYPE { get; } = new DebugCommand<string>("controllerType", "Changes the controller input type", "controllerType", (x) => {
        ControlManager manager;
        if (x == "ps4")
        {
            
            manager = GameObject.Find("Player").GetComponent<ControlManager>();
            manager.playstation = true;

            manager = GameObject.Find("Player2").GetComponent<ControlManager>();
            manager.playstation = true;
        }
        else if (x == "xbx")
        {
            manager = GameObject.Find("Player").GetComponent<ControlManager>();
            manager.playstation = false;

            manager = GameObject.Find("Player2").GetComponent<ControlManager>();
            manager.playstation = false;
        }
    });

    public static DebugCommand<string> EXIT { get; } = new DebugCommand<string>("exit", "Exit the game", "exit", (x) => {
        Application.Quit();
    });


    public static List<object> commandList {get;} = new List<object>
    {
        //Write here your comand too
        LOG,
        LOADSCENE,
        RELOAD,
        CONTROLLERTYPE,
        EXIT,
    };
}
