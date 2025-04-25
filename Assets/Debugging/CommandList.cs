using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandList {
    //Write here your command
    public static DebugCommand LOG {get;} = new DebugCommand("log", "logs", "log", () =>{
            Debug.Log("Console Working Fine");
        });

    
    public static List<object> commandList {get;} = new List<object>
    {
        //Write here your comand too
        LOG,
    };
}
