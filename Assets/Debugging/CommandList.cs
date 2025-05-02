using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
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


    public static List<object> commandList {get;} = new List<object>
    {
        //Write here your comand too
        LOG,
        LOADSCENE,
    };
}
