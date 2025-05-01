using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerCam;
    [SerializeField] Transform ObjPosL;
    [SerializeField] Transform ObjPosR;
    public float ObjDetectionRange = 20f;
    GameObject currentTargetObj;    // object currently in player crosshair
    ThrowableObject leftObject;    // object holding in left
    ThrowableObject rightObject;    // object holding in right
    private bool holdingObjL = false;
    private bool holdingObjR = false;
    [SerializeField] private bool TriggerInUseL = false;
    private bool TriggerInUseR = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // object detection
        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, ObjDetectionRange, LayerMask.GetMask("Object"))){
            if(hit.transform.gameObject != currentTargetObj){
                currentTargetObj = hit.transform.gameObject;
                Debug.Log(hit.transform.name);
            }
        }
        else if(currentTargetObj){
            currentTargetObj = null;
            Debug.Log("target emptied");
        }

        // grab/throw objects
        // Left Trigger
        if(Input.GetAxisRaw("GrabThrowL1") != 0){
            if(!TriggerInUseL){
                // grab
                if(!holdingObjL && currentTargetObj){
                    leftObject = currentTargetObj.GetComponent<ThrowableObject>();
                    if(leftObject.canGrab){
                        Debug.Log("Left grab");
                        leftObject.GrabObject(ObjPosL, playerCam.GetChild(0).transform);
                        holdingObjL = true;
                    }
                }
                // throw
                else if(holdingObjL && leftObject.canThrow){
                    Debug.Log("Left throw");
                    leftObject.ThrowObject();
                    holdingObjL = false;
                }

                TriggerInUseL = true;
            }
        }
        else if(Input.GetAxisRaw("GrabThrowL1") == 0 && TriggerInUseL){
            TriggerInUseL = false;
        }

        // Right Trigger
        if(Input.GetAxisRaw("GrabThrowR1") != 0){
            if(!TriggerInUseR){
                // grab
                if(!holdingObjR && currentTargetObj){
                    rightObject = currentTargetObj.GetComponent<ThrowableObject>();
                    if(rightObject.canGrab){
                        Debug.Log("Right grab");
                        rightObject.GrabObject(ObjPosR, playerCam.GetChild(0).transform);
                        holdingObjR = true;
                    }
                }
                // throw
                else if(holdingObjR && rightObject.canThrow){
                    Debug.Log("Right throw");
                    rightObject.ThrowObject();
                    holdingObjR = false;
                }

                TriggerInUseR = true;
            }
        }
        else if(Input.GetAxisRaw("GrabThrowR1") == 0 && TriggerInUseR){
            TriggerInUseR = false;
        }
    }
}
