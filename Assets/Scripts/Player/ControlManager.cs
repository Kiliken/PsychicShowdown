using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    //public bool playstation = true;
    Player playerScript;
    PlayerMovement playerMovementScript;
    CameraController cameraControllerScript;
    GameSettings gameSettings;

    // P1 PLAYSTATION
    private string moveX1_P = "Horizontal1";
    private string moveY1_P = "Vertical1";
    private string jump1_P = "Jump1";
    private string dash1_P = "Dash1";
    private string sprint1_P = "Sprint1";
    private string camX1_P = "CamX1";
    private string camY1_P = "CamY1";
    private string shldrSwap1_P = "ShoulderSwap1";
    private string grabThrowLeftBtn1_P = "GrabThrowL1";
    private string grabThrowRightBtn1_P = "GrabThrowR1";
    private string aimCancelBtn1_P = "AimCancel1";
    // private string blockL1_P = "BlockL1";
    // private string blockR1_P = "BlockR1";

    // P2 PLAYSTATION
    private string moveX2_P = "Horizontal2";
    private string moveY2_P = "Vertical2";
    private string jump2_P = "Jump2";
    private string dash2_P = "Dash2";
    private string sprint2_P = "Sprint2";
    private string camX2_P = "CamX2";
    private string camY2_P = "CamY2";
    private string shldrSwap2_P = "ShoulderSwap2";
    private string grabThrowLeftBtn2_P = "GrabThrowL2";
    private string grabThrowRightBtn2_P = "GrabThrowR2";
    private string aimCancelBtn2_P = "AimCancel2";
    // private string blockL2_P = "BlockL2";
    // private string blockR2_P = "BlockR2";

    // P1 XBOX
    private string moveX1_X = "Horizontal1";
    private string moveY1_X = "Vertical1";
    private string jump1_X = "Jump1X";
    private string dash1_X = "Dash1X";
    private string sprint1_X = "Sprint1X";
    private string camX1_X = "CamX1X";
    private string camY1_X = "CamY1X";
    private string shldrSwap1_X = "ShoulderSwap1X";
    private string grabThrowLeftBtn1_X = "GrabThrowL1X";
    private string grabThrowRightBtn1_X = "GrabThrowR1X";
    private string aimCancelBtn1_X = "AimCancel1X";
    // private string blockL1_X = "BlockL1X";
    // private string blockR1_X = "BlockR1X";

    // P2 XBOX
    private string moveX2_X = "Horizontal2";
    private string moveY2_X = "Vertical2";
    private string jump2_X = "Jump2X";
    private string dash2_X = "Dash2X";
    private string sprint2_X = "Sprint2X";
    private string camX2_X = "CamX2X";
    private string camY2_X = "CamY2X";
    private string shldrSwap2_X = "ShoulderSwap2X";
    private string grabThrowLeftBtn2_X = "GrabThrowL2X";
    private string grabThrowRightBtn2_X = "GrabThrowR2X";
    private string aimCancelBtn2_X = "AimCancel2X";
    // private string blockL2_X = "BlockL2X";
    // private string blockR2_X = "BlockR2X";


    void Awake(){
        playerScript = GetComponent<Player>();
        playerMovementScript = GetComponent<PlayerMovement>();
        cameraControllerScript = playerScript.playerCam.GetComponent<CameraController>();
        SetControls();
    }


    // Start is called before the first frame update
    void Start()
    {
        string[] cName = Input.GetJoystickNames();
        int currentConnectionCount = 0;
        for (int i = 0; i < cName.Length; i++)
        {
            Debug.Log(cName[i]);
            if(cName[i] != "")
            {
                currentConnectionCount++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetControls(){
        if (playerScript.playerNo == 1)
        {
            if(gameSettings.p1ControllerIsPS)
            {
                Debug.Log("Player 1 Playstation controls");
                playerMovementScript.moveXInput = moveX1_P;
                playerMovementScript.moveYInput = moveY1_P;
                playerMovementScript.jumpBtn = jump1_P;
                playerMovementScript.dashBtn = dash1_P;
                playerMovementScript.sprintBtn = sprint1_P;
                cameraControllerScript.camXInput = camX1_P;
                cameraControllerScript.camYInput = camY1_P;
                cameraControllerScript.shldrSwapBtn = shldrSwap1_P;
                playerScript.grabThrowLeftBtn = grabThrowLeftBtn1_P;
                playerScript.grabThrowRightBtn = grabThrowRightBtn1_P;
                playerScript.aimCancelBtn = aimCancelBtn1_P;
                playerScript.triggerNegative = -1;
            }
            else
            {
                Debug.Log("Player 1 Xbox controls");
                playerMovementScript.moveXInput = moveX1_X;
                playerMovementScript.moveYInput = moveY1_X;
                playerMovementScript.jumpBtn = jump1_X;
                playerMovementScript.dashBtn = dash1_X;
                playerMovementScript.sprintBtn = sprint1_X;
                cameraControllerScript.camXInput = camX1_X;
                cameraControllerScript.camYInput = camY1_X;
                cameraControllerScript.shldrSwapBtn = shldrSwap1_X;
                playerScript.grabThrowLeftBtn = grabThrowLeftBtn1_X;
                playerScript.grabThrowRightBtn = grabThrowRightBtn1_X;
                playerScript.aimCancelBtn = aimCancelBtn1_X;
                playerScript.triggerNegative = 0;
            }
        }
        else
        {
            if (gameSettings.p2ControllerIsPS)
            {
                Debug.Log("Player 2 Playstation controls");
                playerMovementScript.moveXInput = moveX2_P;
                playerMovementScript.moveYInput = moveY2_P;
                playerMovementScript.jumpBtn = jump2_P;
                playerMovementScript.dashBtn = dash2_P;
                playerMovementScript.sprintBtn = sprint2_P;
                cameraControllerScript.camXInput = camX2_P;
                cameraControllerScript.camYInput = camY2_P;
                cameraControllerScript.shldrSwapBtn = shldrSwap2_P;
                playerScript.grabThrowLeftBtn = grabThrowLeftBtn2_P;
                playerScript.grabThrowRightBtn = grabThrowRightBtn2_P;
                playerScript.aimCancelBtn = aimCancelBtn2_P;
                playerScript.triggerNegative = -1;
            }
            else
            {
                Debug.Log("Player 2 Xbox controls");
                playerMovementScript.moveXInput = moveX2_X;
                playerMovementScript.moveYInput = moveY2_X;
                playerMovementScript.jumpBtn = jump2_X;
                playerMovementScript.dashBtn = dash2_X;
                playerMovementScript.sprintBtn = sprint2_X;
                cameraControllerScript.camXInput = camX2_X;
                cameraControllerScript.camYInput = camY2_X;
                cameraControllerScript.shldrSwapBtn = shldrSwap2_X;
                playerScript.grabThrowLeftBtn = grabThrowLeftBtn2_X;
                playerScript.grabThrowRightBtn = grabThrowRightBtn2_X;
                playerScript.aimCancelBtn = aimCancelBtn2_X;
                playerScript.triggerNegative = 0;
            }
        }













        //if(gameSettings.p1ControllerIsPS){
        //    if(playerScript.playerNo == 1){
        //        Debug.Log("Player 1 Playstation controls");
        //        playerMovementScript.moveXInput = moveX1_P;
        //        playerMovementScript.moveYInput = moveY1_P;
        //        playerMovementScript.jumpBtn = jump1_P;
        //        playerMovementScript.dashBtn = dash1_P;
        //        playerMovementScript.sprintBtn = sprint1_P;
        //        cameraControllerScript.camXInput = camX1_P;
        //        cameraControllerScript.camYInput = camY1_P;
        //        cameraControllerScript.shldrSwapBtn = shldrSwap1_P;
        //        playerScript.grabThrowLeftBtn = grabThrowLeftBtn1_P;
        //        playerScript.grabThrowRightBtn = grabThrowRightBtn1_P;
        //        playerScript.aimCancelBtn = aimCancelBtn1_P;
        //    }
        //    else{
        //        Debug.Log("Player 2 Playstation controls");
        //        playerMovementScript.moveXInput = moveX2_P;
        //        playerMovementScript.moveYInput = moveY2_P;
        //        playerMovementScript.jumpBtn = jump2_P;
        //        playerMovementScript.dashBtn = dash2_P;
        //        playerMovementScript.sprintBtn = sprint2_P;
        //        cameraControllerScript.camXInput = camX2_P;
        //        cameraControllerScript.camYInput = camY2_P;
        //        cameraControllerScript.shldrSwapBtn = shldrSwap2_P;
        //        playerScript.grabThrowLeftBtn = grabThrowLeftBtn2_P;
        //        playerScript.grabThrowRightBtn = grabThrowRightBtn2_P;
        //        playerScript.aimCancelBtn = aimCancelBtn2_P;
        //    }
        //    playerScript.triggerNegative = -1;
        //}
        //else{
        //    if(playerScript.playerNo == 1){
        //        Debug.Log("Player 1 Xbox controls");
        //        playerMovementScript.moveXInput = moveX1_X;
        //        playerMovementScript.moveYInput = moveY1_X;
        //        playerMovementScript.jumpBtn = jump1_X;
        //        playerMovementScript.dashBtn = dash1_X;
        //        playerMovementScript.sprintBtn = sprint1_X;
        //        cameraControllerScript.camXInput = camX1_X;
        //        cameraControllerScript.camYInput = camY1_X;
        //        cameraControllerScript.shldrSwapBtn = shldrSwap1_X;
        //        playerScript.grabThrowLeftBtn = grabThrowLeftBtn1_X;
        //        playerScript.grabThrowRightBtn = grabThrowRightBtn1_X;
        //        playerScript.aimCancelBtn = aimCancelBtn1_X;
        //    }
        //    else{
        //        Debug.Log("Player 2 Xbox controls");
        //        playerMovementScript.moveXInput = moveX2_X;
        //        playerMovementScript.moveYInput = moveY2_X;
        //        playerMovementScript.jumpBtn = jump2_X;
        //        playerMovementScript.dashBtn = dash2_X;
        //        playerMovementScript.sprintBtn = sprint2_X;
        //        cameraControllerScript.camXInput = camX2_X;
        //        cameraControllerScript.camYInput = camY2_X;
        //        cameraControllerScript.shldrSwapBtn = shldrSwap2_X;
        //        playerScript.grabThrowLeftBtn = grabThrowLeftBtn2_X;
        //        playerScript.grabThrowRightBtn = grabThrowRightBtn2_X;
        //        playerScript.aimCancelBtn = aimCancelBtn2_X;
        //    }
        //    playerScript.triggerNegative = 0;
        //}
    }
}
