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
        leftObject.GrabObject(shootPos, shootPos, playerNo);
        holdingObjL = true;
        //sfxPlayer.PlaySFX(2);
    }

    public void GrabRightObject(ThrowableObject obj)
    {
        rightObject = obj;
        Debug.Log("Right grab " + obj.objectName);
        rightObject.GrabObject(shootPos, shootPos, playerNo);
        holdingObjR = true;
        //sfxPlayer.PlaySFX(2);
    }

    public void ThrowLeftObject()
    {
        Debug.Log("Left throw");
        leftObject.ThrowObject();
        holdingObjL = false;
        //sfxPlayer.PlaySFX(3);
    }

    public void ThrowRightObject()
    {
        Debug.Log("Right throw");
        rightObject.ThrowObject();
        holdingObjR = false;
        //sfxPlayer.PlaySFX(3);
    }

}
