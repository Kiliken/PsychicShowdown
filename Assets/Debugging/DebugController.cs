using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class DebugController : MonoBehaviour
{
    bool showConsole = false;
    string input = string.Empty;

    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();

    public string sceneName = null;

    void Awake()
    {   
        if(GetArg("-scene") != null)
        {
            sceneName = GetArg("-scene");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backslash))
            showConsole = !showConsole;
        
        if (input.Contains("\n"))
        {
            HandleInput();
            input = "";
            showConsole = false;
        }
            
    }

    private void OnGUI()
    {
        if (!showConsole) 
            return;

        //float y = 0f;

        GUI.Box(new Rect(0, 0, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0,0,0,0);
        input = GUI.TextArea(new Rect(10f, 5f, Screen.width-20f, 30), input);
        GUILayout.BeginArea(new Rect(10f, 30, Screen.width, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        GUILayout.EndArea();    
    }

    private void HandleInput()
    {
        input = input.Substring(0, input.Length-1) + " @";
        string[] proprieties = input.Split(' ');

        for (int i=0; i<CommandList.commandList.Count; i++)
        {
            DebugCommandBase commandBase = CommandList.commandList[i] as DebugCommandBase;

            if(input.Contains(commandBase.commandId))
            {
                if (CommandList.commandList[i] as DebugCommand != null)
                    (CommandList.commandList[i] as DebugCommand).Invoke();
                else if (CommandList.commandList[i] as DebugCommand<string> != null)
                {
                    (CommandList.commandList[i] as DebugCommand<string>).Invoke(proprieties[1]);
                }
            }
            
            
        }
    }


    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
