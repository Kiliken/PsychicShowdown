using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    public int playerNo = 1;    // 1 or 2
    PlayerMovement movementScript;
    PlayerSFXPlayer sfxPlayer;
    ObjHolder objHolder;
    Transform shootPos;
    Transform detectPos;
    public Transform playerCam;   // camera transform
    Camera cam;     // camera
    CameraController camController;
    TextMeshProUGUI objectText; // for displaying the name of the object the player's crosshair is hovering on

    [SerializeField] float defaultZoom = 60f;
    float smallZoom = 60f; // for medium object
    float mediumZoom = 80f; // for medium object
    float largeZoom = 100f; // for large object
    [SerializeField] float adsZoom = 30f;
    [SerializeField] float adsSpeed = 180f; // 120
    [SerializeField] float camSenNormal = 3f;   // camera sensitivity normal
    [SerializeField] float camSenADS = 0.5f;    // camera sensitivity ADS


    [SerializeField] Transform objPosL;
    [SerializeField] Transform objPosR;
    [SerializeField] LayerMask objLayerMask; // both object and obstacle (large objects)
    public PlayerPanel playerPanel;

    public float objDetectionRange = 30f;   // 20 default
    [SerializeField] GameObject currentTargetObj;    // object currently in player crosshair
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

    private float sphereRadius = 2f; // sphere radius for SphereCast


    //temporary player hp (remove after implementing actual hp)
    public int maxHP = 10;
    public int hp = 10;     //cast to sbyte

    public string grabThrowLeftBtn = "GrabThrowL1";
    public string grabThrowRightBtn = "GrabThrowR1";
    public string aimCancelBtn = "AimCancel1";
    public string pauseBtn = "Pause1"; // pause button for player 1
    public int triggerNegative = -1; // ps trigger negative is -1, xbox is 0

    public bool playerActive = true;
    public GameManager gameManager;
    private GameSettings gameSettings;


    InGameMenu pauseMenu; // reference to the pause menu script


    //For online mode
    [NonSerialized]
    public byte leftHandFlag = 0x00;
    [NonSerialized]
    public byte rightHandFlag = 0x00;
    [NonSerialized]
    public ushort leftObjRef = 0;
    [NonSerialized]
    public ushort rightObjRef = 0;

    private void Awake()
    {
        hp = maxHP;
        movementScript = GetComponent<PlayerMovement>();
        if (playerNo == 1)
        {
            movementScript.isP1 = true;

        }
        else if (playerNo == 2)
        {
            movementScript.isP1 = false;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

        //playerPanel.UpdateHPBar();
        // set up player cam in other scripts

        movementScript.playerCam = playerCam;
        objHolder = transform.GetChild(1).GetComponent<ObjHolder>();
        objHolder.playerCam = playerCam;
        shootPos = playerCam.GetChild(0).transform;
        detectPos = playerCam.GetChild(1).transform;
        pauseMenu = GameObject.Find("PauseScreens").GetComponent<InGameMenu>();

        cam = playerCam.gameObject.GetComponent<Camera>();
        camController = playerCam.gameObject.GetComponent<CameraController>();
        camController.RotSpeedX = camController.RotSpeedY = camSenNormal;

        sfxPlayer = GetComponent<PlayerSFXPlayer>();

        if (playerNo == 1)
            objectText = GameObject.Find("GameUI/P1UI/ObjectText").GetComponent<TextMeshProUGUI>();
        else
            objectText = GameObject.Find("GameUI/P2UI/ObjectText").GetComponent<TextMeshProUGUI>();

        defaultZoom = smallZoom;


        gameSettings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
        if (playerNo == 1)
        {
            camSenNormal = 1f + (gameSettings.p1Sensitivity * 4);
            movementScript.isP1 = true;

        }
        else if (playerNo == 2)
        {
            camSenNormal = 1f + (gameSettings.p2Sensitivity * 4);
            movementScript.isP1 = false;
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (!playerActive) return;

        ObjectDetection();
        PlayerInput();
        InGameMenuInput();
        //GetCameraDistance();
    }


    // detect pickable objects in front of player
    private void ObjectDetection()
    {
        // 
        // SPHERECAST VISUALIZATION DEBUG
        Vector3 origin = detectPos.position;
        Vector3 direction = playerCam.transform.forward;
        float distance = objDetectionRange;
        float radius = sphereRadius;

        // For drawing the path of the spherecast
        Debug.DrawRay(origin, direction * distance, Color.red, 0.1f);

        // Draw multiple wire spheres along the cast path
        int numSteps = 10;
        for (int i = 0; i <= numSteps; i++)
        {
            float step = (float)i / numSteps;
            Vector3 point = origin + direction * (distance * step);
            DebugExtension.DebugWireSphere(point, Color.yellow, radius, 0.1f);
        }

        // object detection
        RaycastHit hit;
        if (Physics.SphereCast(detectPos.position, sphereRadius, playerCam.transform.forward, out hit, objDetectionRange, objLayerMask))
        //if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, objDetectionRange, objLayerMask))
        {
            if (hit.collider.gameObject.tag == "Object" && hit.collider.gameObject != currentTargetObj)
            {
                if (currentTargetObj)
                {
                    currentTargetObj.GetComponent<ThrowableObject>().ShowHideHighlight(false);
                }
                currentTargetObj = hit.transform.gameObject;
                if (currentTargetObj.GetComponent<ThrowableObject>().canGrab && (!leftAiming && !rightAiming))
                {
                    currentTargetObj.GetComponent<ThrowableObject>().ShowHideHighlight(true);
                    objectText.text = currentTargetObj.GetComponent<ThrowableObject>().objectName;
                    //Debug.Log(hit.transform.name);
                }
                else
                {
                    currentTargetObj.GetComponent<ThrowableObject>().ShowHideHighlight(false);
                    objectText.text = "";
                }

            }
        }
        else if (currentTargetObj)
        {
            currentTargetObj.GetComponent<ThrowableObject>().ShowHideHighlight(false);
            objectText.text = "";
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


                        leftHandFlag++;
                        leftObjRef = leftObject.objectID;

                        GetCameraDistance();

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

                    if (movementScript.isSprinting) movementScript.isSprinting = false;
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

                //leftHandFlag++ and objId = 0
                leftHandFlag++;
                leftObjRef = 0;

                GetCameraDistance();


                sfxPlayer.PlaySFX(3);

            }

            if (holdingObjR)
                rightObject.ShowHideObject(true, false);

            triggerInUseL = false;
        }
        else if (Mathf.Abs(cam.fieldOfView - defaultZoom) > 0.01f && !rightAiming)
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
                        //rightHandFlag++

                        rightHandFlag++;
                        rightObjRef = rightObject.objectID;
                        

                        GetCameraDistance();

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

                    if (movementScript.isSprinting) movementScript.isSprinting = false;
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
                //rightHandFlag++ and objId = 0
                rightHandFlag++;
                rightObjRef = 0;

                GetCameraDistance();

                sfxPlayer.PlaySFX(3);
            }

            if (holdingObjL)
                leftObject.ShowHideObject(true, false);

            triggerInUseR = false;
        }
        else if (Mathf.Abs(cam.fieldOfView - defaultZoom) > 0.01f && !leftAiming)
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
        playerPanel.UpdateHPBar();
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

    //Activates and runs the in game menu.
    public void InGameMenuInput()
    {
        if (Input.GetButtonDown(pauseBtn))
        {
            Debug.Log("Pause button pressed for player " + playerNo);
            if (gameManager.gameStarted)
            {
                if (pauseMenu.isPlayerPauseMenuActive(playerNo))
                {
                    pauseMenu.HidePauseMenu(playerNo);
                }
                else
                {
                    pauseMenu.ShowPauseMenu(playerNo);
                }
            }
        }

        if (pauseMenu.isPlayerPauseMenuActive(playerNo))
        {
            // If the pause menu is active, disable player movement and camera control
            movementScript.inputActive = false;
            camController.inputActive = false;
            //Debug.Log("set inactive" + playerNo);
        }
        else
        {
            // If the pause menu is not active, enable player movement and camera control
            movementScript.inputActive = true;
            camController.inputActive = true;
            //Debug.Log("set active" + playerNo);
        }
    }

    // Continue the game from the pause menu by enabling player movement and camera control
    public void ContinueGame()
    {
        pauseMenu.HidePauseMenu(playerNo);
        movementScript.inputActive = true;
        camController.inputActive = true;
    }


    public void SetPlayerActive(bool a)
    {
        playerActive = a;
        movementScript.playerActive = a;
        camController.playerActive = a;
    }

    public void GetCameraDistance()
    {
        int l = (leftObject != null && holdingObjL) ? leftObject.objectSize : 0;
        int r = (rightObject != null && holdingObjR) ? rightObject.objectSize : 0;
        int largestSize = Mathf.Max(l, r);
        Debug.Log(largestSize);
        switch (largestSize)
        {
            case 0:
                defaultZoom = smallZoom;
                camController.offsetXTarget = camController.shoulderSwapped ? -4f : 4f;
                objHolder.ChangeHolderPos(0);
                break;
            case 1:
                defaultZoom = mediumZoom;
                camController.offsetXTarget = camController.shoulderSwapped ? -6f : 6f;
                objHolder.ChangeHolderPos(1);
                break;
            case 2:
                defaultZoom = largeZoom;
                camController.offsetXTarget = camController.shoulderSwapped ? -8f : 8f;
                objHolder.ChangeHolderPos(2);
                break;
        }
    }


}
