using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    protected Rigidbody rb;
    protected ObjHitbox hitbox;
    public int objectID = 0;    // ID for showing object on UI, etc.
    [SerializeField] protected int objectSize = 0; // 0:small, 1:medium, 2:large
    public bool canGrab = true;
    public bool canThrow = false;
    public bool aiming = false;
    protected bool thrown = false;

    // THROW SPEEDS
    protected float[] throwSpeeds = new float[]{120f, 80f, 50f}; // S, M, L
    [SerializeField] protected float throwSpeed = 150f;
    // GRAB SPEEDS
    protected float[] grabSpeeds = new float[]{30f, 20f, 10f}; // S, M, L
    [SerializeField] protected float grabSpeed = 30f;

    public Transform grabbedTransform;
    public Transform shootPos;
    public int holdingPlayer = 0;


    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
        hitbox = transform.GetChild(0).GetComponent<ObjHitbox>();
        throwSpeed = throwSpeeds[objectSize];
        grabSpeed = grabSpeeds[objectSize];
    }


    // Update is called once per frame
    void Update(){
        if(aiming){
            transform.position = shootPos.position;
            transform.rotation = grabbedTransform.parent.transform.rotation;
        }
        else if(!thrown && !canGrab){
            if(Vector3.Distance(transform.position, grabbedTransform.position) > 0.1f){
                transform.position = Vector3.MoveTowards(transform.position, grabbedTransform.position,grabSpeed * Time.deltaTime);
            }
            else{
                transform.position = grabbedTransform.position;
                transform.rotation = grabbedTransform.parent.transform.rotation;
                canThrow = true;
            }
        }
    }


    public void GrabObject(Transform posTransform, Transform shootTransform, int player){
        grabbedTransform = posTransform;
        shootPos = shootTransform;
        holdingPlayer = player;
        rb.useGravity = false;
        GetComponent<MeshCollider>().enabled = false;
        canGrab = false;
    }


    public void ThrowObject(){
        thrown = true;
        aiming = false;
        rb.useGravity = true;
        GetComponent<MeshCollider>().enabled = true;
        // activate hit box
        hitbox.ActivateHitbox(holdingPlayer);
        rb.AddForce(grabbedTransform.parent.transform.forward * throwSpeed, ForceMode.Impulse);
    }


    private void OnCollisionEnter(Collision collision){
        if(thrown){
            if(collision.gameObject.tag == "Ground"){
                // break or effect
                // ObjectEffect();
            }
        }
    }


    // override with new script
    public virtual void ObjectEffect(){
        //Destroy(this.gameObject);
    }
}
