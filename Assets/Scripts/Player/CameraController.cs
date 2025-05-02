using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;    // player transform
    [SerializeField] float camDistance = 7f;
    [SerializeField] float camVertAngleMin = -45f;
    [SerializeField] float camVertAngleMax = 45f;
    [SerializeField] float offsetX = 0f;
    [SerializeField] float offsetY = 2f;
    [SerializeField] float RotSpeedX = 2f;
    [SerializeField] float RotSpeedY = 2f;
    private float rotationX;
    private float rotationY;
    [SerializeField] bool invertX = false;
    [SerializeField] bool invertY = false;
    private float invertXVal = 1f;
    private float invertYVal = 1f;


    // Start is called before the first frame update
    void Start(){
        
    }


    // Update is called once per frame
    void Update(){
        invertXVal = invertX ? -1 : 1;
        invertYVal = invertY ? -1 : 1;
        
        // diagonal input
        if(Input.GetAxisRaw("Cam1X") != 0 && Input.GetAxisRaw("Cam1Y") != 0){
            rotationX += Input.GetAxisRaw("Cam1Y") * invertYVal * RotSpeedX * 1.6f;
            rotationX = Mathf.Clamp(rotationX, camVertAngleMin, camVertAngleMax); // clamp vertical angle to not move all the way
            rotationY += Input.GetAxisRaw("Cam1X") * invertXVal *  RotSpeedY * 1.6f;
        }
        // normal input
        else{
            rotationX += Input.GetAxis("Cam1Y") * invertYVal * RotSpeedX;
            rotationX = Mathf.Clamp(rotationX, camVertAngleMin, camVertAngleMax); // clamp vertical angle to not move all the way
            rotationY += Input.GetAxis("Cam1X") * invertXVal *  RotSpeedY;
        }
        
        // if(Input.GetAxis("Cam1X") != 0 || Input.GetAxis("Cam1Y") != 0){
        //     Debug.Log("X: " + Input.GetAxis("Cam1X") + " Y: " + Input.GetAxis("Cam1Y"));
        // }

        // third-person shoulder swap
        if(Input.GetButtonDown("ShoulderSwap1")){
            offsetX = -offsetX;
        }

        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 pivotOffset = targetRotation * new Vector3(offsetX, offsetY, 0);    // include offset
        Vector3 focusPosition = followTarget.position + pivotOffset;

        transform.position = focusPosition - targetRotation * new Vector3(0, 0, camDistance);
        transform.rotation = targetRotation;
    }
}
