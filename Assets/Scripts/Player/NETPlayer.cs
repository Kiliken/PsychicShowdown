using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class NETPlayer : MonoBehaviour
{
    public int playerNo = 1;    // 1 or 2
    Rigidbody rb;
    PlayerSFXPlayer sfxPlayer;
    ObjHolder objHolder;
    [SerializeField] Transform shootPos;
    public Transform playerCam;   // camera transform

    [SerializeField] Animator playerAnimator;

    public Transform tempShootPos;
    [SerializeField] Transform objPosL;
    [SerializeField] Transform objPosR;


    [SerializeField] ThrowableObject leftObject;    // object holding in left
    [SerializeField] ThrowableObject rightObject;    // object holding in right
    private bool holdingObjL = false;
    private bool holdingObjR = false;


    //temporary player hp (remove after implementing actual hp)
    public int maxHP = 10;
    public int hp = 10;     //cast to sbyte

    public bool playerActive = true;
    public GameManager gameManager;

    // Effects
    [SerializeField] GameObject jumpEffect;


    //For online mode
    [NonSerialized]
    public byte leftHandFlag;
    [NonSerialized]
    public byte rightHandFlag;
    [NonSerialized]
    public ushort leftObjRef;
    [NonSerialized]
    public ushort rightObjRef;

    private void Awake()
    {
        hp = maxHP;
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerAnimator = transform.GetChild(0).GetChild(1).GetComponent<Animator>();
        tempShootPos = transform.GetChild(3).transform;

        objHolder = transform.GetChild(1).GetComponent<ObjHolder>();
        objPosL = objHolder.transform.GetChild(0).transform;
        objPosR = objHolder.transform.GetChild(1).transform;

        objHolder.playerCam = tempShootPos;
        shootPos = tempShootPos;

        sfxPlayer = GetComponent<PlayerSFXPlayer>();
        jumpEffect = Resources.Load<GameObject>("Prefabs/Particles/JumpV");
    }


    // Update is called once per frame
    void Update()
    {
        if (!playerActive) return;


    }

    public void UpdatePosition(float newX, float newY, float newZ, float rotBody)
    {
        //rb.position = Vector3.Lerp(rb.position, new Vector3(newX, newY, newZ), Time.deltaTime * 10f);
        Vector3 newPos = new Vector3(newX, newY, newZ);
        Vector3 oldPos = rb.position;  // last frame position

        // movement vector in world space
        Vector3 moveDir = (newPos - oldPos) / Time.deltaTime;
        float speed = moveDir.magnitude;
        Debug.Log("net player velocity: " + speed);

        // If speed is very small, treat as not moving
        if (speed < 0.1f)   // threshold for idle
        {
            playerAnimator.SetFloat("PosX", 0f);
            playerAnimator.SetFloat("PosY", 0f);
        }
        else
        {
            // world space to local
            Vector3 localDir = transform.InverseTransformDirection(moveDir.normalized);

            int moveX = Mathf.RoundToInt(Mathf.Clamp(localDir.x * speed, -1f, 1f));
            int moveZ = Mathf.RoundToInt(Mathf.Clamp(localDir.z * speed, -1f, 1f));
            Debug.Log("x: " + moveX + " z: " + moveZ);

            // set animations
            playerAnimator.SetFloat("PosX", moveX);
            playerAnimator.SetFloat("PosY", moveZ);
        }

        transform.GetChild(0).eulerAngles = new Vector3(0, rotBody, 0);
    }


    public void GrabLeftObject(ThrowableObject obj)
    {
        leftObject = obj;
        Debug.Log("Left grab " + obj.objectName);
        leftObject.GrabObject(objPosL, shootPos, playerNo);
        holdingObjL = true;
        playerAnimator.SetTrigger("L_Grab");
    }

    public void GrabRightObject(ThrowableObject obj)
    {
        rightObject = obj;
        Debug.Log("Right grab " + obj.objectName);
        rightObject.GrabObject(objPosR, shootPos, playerNo);
        holdingObjR = true;
        playerAnimator.SetTrigger("R_Grab");
        //sfxPlayer.PlaySFX(2);
    }

    public void ThrowLeftObject()
    {
        Debug.Log("Left throw");
        leftObject.ThrowObjectNet();
        holdingObjL = false;
        playerAnimator.SetTrigger("L_Throw");
        //sfxPlayer.PlaySFX(3);
    }

    public void ThrowRightObject()
    {
        Debug.Log("Right throw");
        rightObject.ThrowObjectNet();
        holdingObjR = false;
        playerAnimator.SetTrigger("R_Throw");
        //sfxPlayer.PlaySFX(3);
    }


    public void UpdateHP(sbyte newHP)
    {
        if (!playerActive || (int)newHP == hp) return;

        //UPDATE UI
        //playerPanel.UpdateHPBar();

        hp = (int)newHP;
        Debug.Log("Net Player HP: " + hp);

        // disable hurtbox for splitsecond
        // player dead
        if (hp <= 0)
        {
            if (playerNo == 1)
                gameManager.GameOver(2);
            else
                gameManager.GameOver(1);

            Debug.Log("Net Player dead.");
        }
    }


    public void PlaySFXEffect(sbyte effectNo)
    {
        switch (effectNo)
        {
            // cancel
            case 0x00:
                break;
            // jump
            case 0x01:
                sfxPlayer.PlaySFX(0);
                Instantiate(jumpEffect, transform.position, Quaternion.identity);
                break;
            // dash
            case 0x02:
                sfxPlayer.PlaySFX(1);
                break;
            // pick up
            case 0x03:
                sfxPlayer.PlaySFX(2);
                break;
            // throw
            case 0x04:
                sfxPlayer.PlaySFX(3);
                break;
        }
    }

}
