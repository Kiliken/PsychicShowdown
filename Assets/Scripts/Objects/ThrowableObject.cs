using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public bool canGrab = true;
    public bool canThrow = false;
    public bool inPosition = false;
    private bool thrown = false;
    public float throwSpeed = 50f;
    private float grabMoveSpeed = 10f;
    public Transform grabbedTransform;
    public Transform shootPos;
    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!thrown){
            if(!canGrab && !inPosition){
                if(Vector3.Distance(transform.position, grabbedTransform.position) > 0.1f){
                    transform.position = Vector3.MoveTowards(transform.position, grabbedTransform.position,grabMoveSpeed * Time.deltaTime);
                }
                else{
                    inPosition = true;
                }
            }
            else if(inPosition){
                // move along player
                transform.position = grabbedTransform.position;
                transform.rotation = grabbedTransform.parent.transform.rotation;

                canThrow = true;
            }
        }
        
        
    }

    public void GrabObject(Transform posTransform, Transform shootTransform){
        grabbedTransform = posTransform;
        shootPos = shootTransform;
        rb.useGravity = false;
        canGrab = false;
    }

    public void ThrowObject(){
        inPosition = false;
        transform.position = shootPos.position;
        rb.useGravity = true;
        thrown = true;
        rb.AddForce(grabbedTransform.parent.transform.forward * throwSpeed, ForceMode.Impulse);
    }
}
