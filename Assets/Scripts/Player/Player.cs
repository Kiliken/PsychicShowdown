using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNo = 1;    // 1 or 2
    PlayerMovement movementScript;
    PlayerSFXPlayer sfxPlayer;
    ObjHolder objHolder;
    Transform shootPos;
    public Transform playerCam;   // camera transform
    Camera cam;     // camera
    CameraController camController;

    [SerializeField] float defaultZoom = 60f;
    [SerializeField] float adsZoom = 30f;
    [SerializeField] float adsSpeed = 120f;
    [SerializeField] float camSenNormal = 3f;   // camera sensitivity normal
    [SerializeField] float camSenADS = 0.5f;    // camera sensitivity ADS


    [SerializeField] Transform objPosL;
    [SerializeField] Transform objPosR;
    [SerializeField] LayerMask objLayerMask; // both object and obstacle (large objects)
    public CellHPBar hpBar;

    public float objDetectionRange = 20f;
    GameObject currentTargetObj;    // object currently in player crosshair
    [SerializeField] ThrowableObject leftObject;    // object holding in left
    [SerializeField] ThrowableObject rightObject;    // object holding in right
    private bool holdingObjL = false;
    private bool holdingObjR = false;
    private bool triggerInUseL = false; // checking if triggers are being pressed
    private bool triggerInUseR = false;
    private bool leftAiming = false;
    private bool rightAiming = false;
    private bool aimCanceledL = false;
    private bool aimCanceledR = false;

    //temporary player hp (remove after implementing actual hp)
    public int maxHP = 10;
    public int hp = 10;

    public string grabThrowLeftBtn = "GrabThrowL1";
    public string grabThrowRightBtn = "GrabThrowR1";
    public string aimCancelBtn = "AimCancel1";
    public int triggerNegative = -1; // ps trigger negative is -1, xbox is 0

    public bool playerActive = true;
    public GameManager gameManager;


    private void Awake()
    {
        hp = maxHP;
        hpBar.UpdateHPBar();
    }


    // Start is called before the first frame update
    void Start()
    {
        // set up player cam in other scripts
        movementScript = GetComponent<PlayerMovement>();
        movementScript.playerCam = playerCam;
        objHolder = transform.GetChild(1).GetComponent<ObjHolder>();
        objHolder.playerCam = playerCam;
        shootPos = playerCam.GetChild(0).transform;

        cam = playerCam.gameObject.GetComponent<Camera>();
        camController = playerCam.gameObject.GetComponent<CameraController>();

        sfxPlayer = GetComponent<PlayerSFXPlayer>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!playerActive) return;

        ObjectDetection();
        PlayerInput();
    }


    // detect pickable objects in front of player
    private void ObjectDetection()
    {
        // object detection
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, objDetectionRange, objLayerMask))
        {
            if (hit.transform.gameObject.tag == "Object" && hit.transform.gameObject != currentTargetObj)
            {
                currentTargetObj = hit.transform.gameObject;
                Debug.Log(hit.transform.name);
            }
        }
        else if (currentTargetObj)
        {
            currentTargetObj = null;
            Debug.Log("target emptied");
        }
    }


    // grab/throw objects
    private void PlayerInput()
    {
        //Debug.Log(Input.GetAxisRaw(grabThrowLeftBtn));
        // Left Trigger 
        // xbox axis is 0
        if (Input.GetAxisRaw(grabThrowLeftBtn) != triggerNegative && !aimCanceledL)
        {
            if (!triggerInUseL && !rightAiming)
            {
                // grab
                if (!holdingObjL && currentTargetObj)
                {
                    leftObject = currentTargetObj.GetComponent<ThrowableObject>();
                    if (leftObject.canGrab)
                    {
                        Debug.Log("Left grab");
                        leftObject.GrabObject(objPosL, shootPos, playerNo);
                        holdingObjL = true;

                        sfxPlayer.PlaySFX(2);

                    }
                }
                // aim
                else if (holdingObjL && leftObject.canThrow)
                {
                    Debug.Log("Left aim");
                    leftObject.aiming = true;
                    camController.RotSpeedX = camController.RotSpeedY = camSenADS;
                    if (holdingObjR)
                        rightObject.ShowHideObject(false, false);
                }

                triggerInUseL = true;
            }

            if (holdingObjL && leftObject.aiming && !rightAiming)
            {
                if (Input.GetButtonDown(aimCancelBtn))
                {
                    aimCanceledL = true;
                    leftObject.aiming = false;
                    leftObject.ShowHideObject(true, true);
                    if (holdingObjR)
                        rightObject.ShowHideObject(true, false);
                }
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, adsZoom, adsSpeed * Time.deltaTime);
                leftAiming = true;
            }

        }
        else if (Input.GetAxisRaw(grabThrowLeftBtn) == triggerNegative && triggerInUseL)
        {
            if (aimCanceledL)
            {
                aimCanceledL = false;
            }
            // throw
            else if (holdingObjL && leftObject.canThrow && leftObject.aiming)
            {
                Debug.Log("Left throw");
                leftObject.ThrowObject();
                holdingObjL = false;

                sfxPlayer.PlaySFX(3);

            }

            if (holdingObjR)
                rightObject.ShowHideObject(true, false);

            triggerInUseL = false;
        }
        else if (cam.fieldOfView < defaultZoom && !rightAiming)
        {
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, defaultZoom, adsSpeed * Time.deltaTime);
            leftAiming = false;
            camController.RotSpeedX = camController.RotSpeedY = camSenNormal;
        }

        // Right Trigger
        if (Input.GetAxisRaw(grabThrowRightBtn) != triggerNegative && !aimCanceledR)
        {
            if (!triggerInUseR && !leftAiming)
            {
                // grab
                if (!holdingObjR && currentTargetObj)
                {
                    rightObject = currentTargetObj.GetComponent<ThrowableObject>();
                    if (rightObject.canGrab)
                    {
                        Debug.Log("Right grab");
                        rightObject.GrabObject(objPosR, shootPos, playerNo);
                        holdingObjR = true;

                        sfxPlayer.PlaySFX(2);

                    }
                }
                // aim
                else if (holdingObjR && rightObject.canThrow)
                {
                    Debug.Log("Right aim");
                    rightObject.aiming = true;
                    camController.RotSpeedX = camController.RotSpeedY = camSenADS;
                    if (holdingObjL)
                        leftObject.ShowHideObject(false, false);
                }

                triggerInUseR = true;
            }

            if (holdingObjR && rightObject.aiming && !leftAiming)
            {
                if (Input.GetButtonDown(aimCancelBtn))
                {
                    aimCanceledR = true;
                    rightObject.aiming = false;
                    rightObject.ShowHideObject(true, true);
                    if (holdingObjL)
                        leftObject.ShowHideObject(true, false);
                }
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, adsZoom, adsSpeed * Time.deltaTime);
                rightAiming = true;
            }
        }
        else if (Input.GetAxisRaw(grabThrowRightBtn) == triggerNegative && triggerInUseR)
        {
            if (aimCanceledR)
            {
                aimCanceledR = false;
            }
            // throw
            else if (holdingObjR && rightObject.canThrow && rightObject.aiming)
            {
                Debug.Log("Right throw");
                rightObject.ThrowObject();
                holdingObjR = false;

                sfxPlayer.PlaySFX(3);
            }

            if (holdingObjL)
                leftObject.ShowHideObject(true, false);

            triggerInUseR = false;
        }
        else if (cam.fieldOfView < defaultZoom && !leftAiming)
        {
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, defaultZoom, adsSpeed * Time.deltaTime);
            rightAiming = false;
            camController.RotSpeedX = camController.RotSpeedY = camSenNormal;
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (!playerActive) return;

        //take damage
        hp = Mathf.Max(0, Mathf.Min(hp - damage, maxHP));
        hpBar.UpdateHPBar();
        Debug.Log("Player " + playerNo + " received " + damage + " damage.");

        // disable hurtbox for splitsecond
        // player dead
        if (hp <= 0)
        {
            if (playerNo == 1)
                gameManager.GameOver(2);
            else
                gameManager.GameOver(1);

            Debug.Log("Player " + playerNo + " dead.");
        }
    }

    public void SetPlayerActive(bool a)
    {
        playerActive = a;
        movementScript.playerActive = a;
        camController.playerActive = a;
    }
}
