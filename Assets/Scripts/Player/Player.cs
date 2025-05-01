using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNo = 1;    // 1 or 2
    PlayerMovement movementScript;
    ObjHolder objHolder;
    Transform shootPos;
    [SerializeField] Transform playerCam;
    [SerializeField] Transform objPosL;
    [SerializeField] Transform objPosR;
    public float objDetectionRange = 20f;
    GameObject currentTargetObj;    // object currently in player crosshair
    ThrowableObject leftObject;    // object holding in left
    ThrowableObject rightObject;    // object holding in right
    private bool holdingObjL = false;
    private bool holdingObjR = false;
    private bool triggerInUseL = false; // checking if triggers are being pressed
    private bool triggerInUseR = false;


    // Start is called before the first frame update
    void Start(){
        // set up player cam in other scripts
        movementScript = GetComponent<PlayerMovement>();
        movementScript.playerCam = playerCam;
        objHolder = transform.GetChild(1).GetComponent<ObjHolder>();
        objHolder.playerCam = playerCam;
        shootPos = playerCam.GetChild(0).transform;

    }


    // Update is called once per frame
    void Update(){
        ObjectDetection();
        PlayerInput();
    }


    // detect pickable objects in front of player
    private void ObjectDetection(){
        // object detection
        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, objDetectionRange, LayerMask.GetMask("Object"))){
            if(hit.transform.gameObject != currentTargetObj){
                currentTargetObj = hit.transform.gameObject;
                Debug.Log(hit.transform.name);
            }
        }
        else if(currentTargetObj){
            currentTargetObj = null;
            Debug.Log("target emptied");
        }
    }


    // grab/throw objects
    private void PlayerInput(){
        // Left Trigger
        if(Input.GetAxisRaw("GrabThrowL1") != 0){
            if(!triggerInUseL){
                // grab
                if(!holdingObjL && currentTargetObj){
                    leftObject = currentTargetObj.GetComponent<ThrowableObject>();
                    if(leftObject.canGrab){
                        Debug.Log("Left grab");
                        leftObject.GrabObject(objPosL, shootPos, playerNo);
                        holdingObjL = true;
                    }
                }
                // aim
                else if(holdingObjL && leftObject.canThrow){
                    Debug.Log("Left aim");
                    leftObject.aiming = true;
                }

                triggerInUseL = true;
            }

        }
        else if(Input.GetAxisRaw("GrabThrowL1") == 0 && triggerInUseL){
            // throw
            if(holdingObjL && leftObject.canThrow && leftObject.aiming){
                Debug.Log("Left throw");
                leftObject.ThrowObject();
                holdingObjL = false;
            }
            triggerInUseL = false;
        }

        // Right Trigger
        if(Input.GetAxisRaw("GrabThrowR1") != 0){
            if(!triggerInUseR){
                // grab
                if(!holdingObjR && currentTargetObj){
                    rightObject = currentTargetObj.GetComponent<ThrowableObject>();
                    if(rightObject.canGrab){
                        Debug.Log("Right grab");
                        rightObject.GrabObject(objPosR, shootPos, playerNo);
                        holdingObjR = true;
                    }
                }
                // aim
                else if(holdingObjR && rightObject.canThrow){
                    Debug.Log("Right aim");
                    rightObject.aiming = true;
                }

                triggerInUseR = true;
            }
        }
        else if(Input.GetAxisRaw("GrabThrowR1") == 0 && triggerInUseR){
            // throw
            if(holdingObjR && rightObject.canThrow && rightObject.aiming){
                Debug.Log("Right throw");
                rightObject.ThrowObject();
                holdingObjR = false;
            }
            triggerInUseR = false;
        }
    }
}
