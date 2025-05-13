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
    public float RotSpeedX = 0.1f;
    public float RotSpeedY = 0.1f;
    private float rotationX;
    private float rotationY;
    [SerializeField] bool invertX = false;
    [SerializeField] bool invertY = false;
    private float invertXVal = 1f;
    private float invertYVal = 1f;
    // camera collision
    [SerializeField] LayerMask cameraCollisionLayers; 
    [SerializeField] float cameraCollisionRadius = 0.3f;    // sphere cast radius
    [SerializeField] float cameraMinDistance = 1f;  // minimum distance from player

    public string camXInput = "CamX1";
    public string camYInput = "CamY1";
    public string shldrSwapBtn = "ShoulderSwap1";


    // Start is called before the first frame update
    void Start(){
        
    }


    // Update is called once per frame
    void Update(){
        invertXVal = invertX ? -1 : 1;
        invertYVal = invertY ? -1 : 1;
        
        // // diagonal input
        // if(Input.GetAxisRaw(camXInput) != 0 && Input.GetAxisRaw(camYInput) != 0){
        //     rotationX += Input.GetAxisRaw(camYInput) * invertYVal * RotSpeedX * 1.6f;
        //     rotationX = Mathf.Clamp(rotationX, camVertAngleMin, camVertAngleMax); // clamp vertical angle to not move all the way
        //     rotationY += Input.GetAxisRaw(camXInput) * invertXVal *  RotSpeedY * 1.6f;
        // }
        // // normal input
        // else{
        //     rotationX += Input.GetAxis(camYInput) * invertYVal * RotSpeedX;
        //     rotationX = Mathf.Clamp(rotationX, camVertAngleMin, camVertAngleMax); // clamp vertical angle to not move all the way
        //     rotationY += Input.GetAxis(camXInput) * invertXVal *  RotSpeedY;
        // }

        //increase speed if diagonal input
        float diagonalSpd = 1f;
        if(Input.GetAxisRaw(camXInput) != 0 && Input.GetAxisRaw(camYInput) != 0)
            diagonalSpd = 1.5f;

        rotationX += Input.GetAxis(camYInput) * invertYVal * RotSpeedX * diagonalSpd;
        rotationX = Mathf.Clamp(rotationX, camVertAngleMin, camVertAngleMax); // clamp vertical angle to not move all the way
        rotationY += Input.GetAxis(camXInput) * invertXVal *  RotSpeedY * diagonalSpd;
        
        
        // if(Input.GetAxis("Cam1X") != 0 || Input.GetAxis("Cam1Y") != 0){
        //     Debug.Log("X: " + Input.GetAxis("Cam1X") + " Y: " + Input.GetAxis("Cam1Y"));
        // }

        // third-person shoulder swap
        if(Input.GetButtonDown(shldrSwapBtn)){
            offsetX = -offsetX;
        }

        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 pivotOffset = targetRotation * new Vector3(offsetX, offsetY, 0);    // include offset
        Vector3 focusPosition = followTarget.position + pivotOffset;

        //transform.position = focusPosition - targetRotation * new Vector3(0, 0, camDistance);

        Vector3 desiredCameraPos = focusPosition - targetRotation * new Vector3(0, 0, camDistance);
        Vector3 direction = desiredCameraPos - focusPosition;
        float targetDistance = camDistance;

        if (Physics.SphereCast(focusPosition, cameraCollisionRadius, direction.normalized, out RaycastHit hit, camDistance, cameraCollisionLayers))
        {
            targetDistance = Mathf.Clamp(hit.distance, cameraMinDistance, camDistance);
        }

        // Final camera position after applying  camera collision
        transform.position = focusPosition - targetRotation * new Vector3(0, 0, targetDistance);

        transform.rotation = targetRotation;
    }
}
