using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjHolder : MonoBehaviour
{
    public Transform playerCam;
    GameObject leftHand;
    GameObject rightHand;
    Vector2 smallPos = new Vector2(1.8f, 0);
    Vector2 mediumPos = new Vector2(2f, 0);
    Vector2 largePos = new Vector2(3f, 2f);


    // Start is called before the first frame update
    void Start()
    {
        leftHand = transform.GetChild(0).gameObject;
        rightHand = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = playerCam.rotation;
    }

    public void ChangeHolderPos(int size)
    {
        switch (size)
        {
            case 0:
                leftHand.transform.localPosition = new Vector3(-smallPos.x, smallPos.y, 0);
                rightHand.transform.localPosition = new Vector3(smallPos.x, smallPos.y, 0);
                break;
            case 1:
                leftHand.transform.localPosition = new Vector3(-mediumPos.x, mediumPos.y, 0);
                rightHand.transform.localPosition = new Vector3(mediumPos.x, mediumPos.y, 0);
                break;
            case 2:
                leftHand.transform.localPosition = new Vector3(-largePos.x, largePos.y, 0);
                rightHand.transform.localPosition = new Vector3(largePos.x, largePos.y, 0);
                break;
        }
    }
}
