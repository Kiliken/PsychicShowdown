using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    public bool playstation = true;
    Player playerScript;
    PlayerMovement playerMovementScript;
    CameraController cameraControllerScript;

    // P1 PLAYSTATION
    private string moveX1_P = "Horizontal";
    private string moveY1_P = "Vertical";
    private string jump1_P = "Jump";
    private string dash1_P = "Dash1";
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
    private string camX2_P = "CamX2";
    private string camY2_P = "CamY2";
    private string shldrSwap2_P = "ShoulderSwap2";
    private string grabThrowLeftBtn2_P = "GrabThrowL2";
    private string grabThrowRightBtn2_P = "GrabThrowR2";
    private string aimCancelBtn2_P = "AimCancel2";
    // private string blockL2_P = "BlockL2";
    // private string blockR2_P = "BlockR2";


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
        if(playstation){
            if(playerScript.playerNo == 1){
                Debug.Log("Player 1 Playstation controls");
                playerMovementScript.moveXInput = moveX1_P;
                playerMovementScript.moveYInput = moveY1_P;
                playerMovementScript.jumpBtn = jump1_P;
                playerMovementScript.dashBtn = dash1_P;
                cameraControllerScript.camXInput = camX1_P;
                cameraControllerScript.camYInput = camY1_P;
                cameraControllerScript.shldrSwapBtn = shldrSwap1_P;
                playerScript.grabThrowLeftBtn = grabThrowLeftBtn1_P;
                playerScript.grabThrowRightBtn = grabThrowRightBtn1_P;
                playerScript.aimCancelBtn = aimCancelBtn1_P;
            }
            else{
                Debug.Log("Player 2 Playstation controls");
                playerMovementScript.moveXInput = moveX2_P;
                playerMovementScript.moveYInput = moveY2_P;
                playerMovementScript.jumpBtn = jump2_P;
                playerMovementScript.dashBtn = dash2_P;
                cameraControllerScript.camXInput = camX2_P;
                cameraControllerScript.camYInput = camY2_P;
                cameraControllerScript.shldrSwapBtn = shldrSwap2_P;
                playerScript.grabThrowLeftBtn = grabThrowLeftBtn2_P;
                playerScript.grabThrowRightBtn = grabThrowRightBtn2_P;
                playerScript.aimCancelBtn = aimCancelBtn2_P;
            }
        }
    }
}
