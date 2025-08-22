using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class NETPlayer : MonoBehaviour
{
    public int playerNo = 1;    // 1 or 2
    PlayerSFXPlayer sfxPlayer;
    ObjHolder objHolder;
    [SerializeField] Transform shootPos;
    public Transform playerCam;   // camera transform

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
        tempShootPos = transform.GetChild(3).transform;

        objHolder = transform.GetChild(1).GetComponent<ObjHolder>();
        objPosL = objHolder.transform.GetChild(0).transform;
        objPosR = objHolder.transform.GetChild(1).transform;

        objHolder.playerCam = tempShootPos;
        shootPos = tempShootPos;

        sfxPlayer = GetComponent<PlayerSFXPlayer>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!playerActive) return;


    }

    public void GrabLeftObject(ThrowableObject obj)
    {
        leftObject = obj;
        Debug.Log("Left grab " + obj.objectName);
        leftObject.GrabObject(objPosL, shootPos, playerNo);
        holdingObjL = true;
    }

    public void GrabRightObject(ThrowableObject obj)
    {
        rightObject = obj;
        Debug.Log("Right grab " + obj.objectName);
        rightObject.GrabObject(objPosR, shootPos, playerNo);
        holdingObjR = true;
        //sfxPlayer.PlaySFX(2);
    }

    public void ThrowLeftObject()
    {
        Debug.Log("Left throw");
        leftObject.ThrowObjectNet();
        holdingObjL = false;
        //sfxPlayer.PlaySFX(3);
    }

    public void ThrowRightObject()
    {
        Debug.Log("Right throw");
        rightObject.ThrowObjectNet();
        holdingObjR = false;
        //sfxPlayer.PlaySFX(3);
    }


    public void PlaySFXEffect(sbyte effectNo)
    {
        switch(effectNo)
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
