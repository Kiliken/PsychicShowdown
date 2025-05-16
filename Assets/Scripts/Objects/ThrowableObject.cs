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
    protected bool grabbed = false;
    public bool canThrow = false;
    public bool aiming = false;
    protected bool thrown = false;
    protected bool effectActivated = false;
    // DAMAGES
    protected int[] damages = new int[]{10, 15, 20}; // S, M, L
    [SerializeField] public int damage = 10;

    // THROW SPEEDS
    protected float[] throwSpeeds = new float[]{120f, 80f, 50f}; // S, M, L
    [SerializeField] protected float throwSpeed = 120f;
    // GRAB SPEEDS
    protected float[] grabSpeeds = new float[]{30f, 20f, 10f}; // S, M, L
    [SerializeField] protected float grabSpeed = 30f;

    public Transform grabbedTransform;
    public Transform shootPos;
    public int holdingPlayer = 0;
    [SerializeField] Vector3 grabbedRotation;
    [SerializeField] Vector3 shootRotation;
    
    [SerializeField] protected float disableHitboxVelo = 20f;    // the magnitude of the velocity to disable the hitbox when thrown
    protected bool objectDisabled = false;
    [SerializeField] protected float destroyAfterSec = 3f;
    protected float destroyTimer = 0f;



    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
        hitbox = transform.GetChild(0).GetComponent<ObjHitbox>();
        damage = damages[objectSize];
        throwSpeed = throwSpeeds[objectSize];
        grabSpeed = grabSpeeds[objectSize];
    }


    // Update is called once per frame
    void Update(){
        // aim
        if(aiming){
            transform.position = shootPos.position;
            transform.rotation = grabbedTransform.parent.rotation * Quaternion.Euler(shootRotation.x, shootRotation.y, shootRotation.z);
            if(GetComponent<MeshRenderer>().enabled)
                GetComponent<MeshRenderer>().enabled = false;
        }
        else if(!thrown && !canGrab){
            // grab
            if(!grabbed && Vector3.Distance(transform.position, grabbedTransform.position) > 0.1f){
                transform.position = Vector3.MoveTowards(transform.position, grabbedTransform.position,grabSpeed * Time.deltaTime);
            }
            // snap to player
            else{
                transform.position = grabbedTransform.position;
                transform.rotation = grabbedTransform.parent.rotation * Quaternion.Euler(grabbedRotation.x, grabbedRotation.y, grabbedRotation.z);
                canThrow = true;
                grabbed = true;
            }
        }
        // if thrown and low velocity, disable hitbox and collider
        else if(thrown && rb.velocity.magnitude > 0 && rb.velocity.magnitude < disableHitboxVelo){
            DisableObject();
        }
        // set object to be destoryed after collision/effect activation
        if(effectActivated){
            if(destroyTimer < destroyAfterSec){
                destroyTimer += Time.deltaTime;
            }
            else{
                Destroy(this.gameObject);
            }
        }
        //Debug.Log(rb.velocity.magnitude);
    }


    public void GrabObject(Transform posTransform, Transform shootTransform, int player){
        if(objectSize == 2){
            rb.isKinematic = false;
            GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Object");
        }
        
        grabbedTransform = posTransform;
        shootPos = shootTransform;
        holdingPlayer = player;
        rb.useGravity = false;
        GetComponent<MeshCollider>().enabled = false;
        canGrab = false;
    }


    public void ThrowObject(){
        aiming = false;
        rb.useGravity = true;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
        GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Object");
        // activate hit box
        hitbox.ActivateHitbox(holdingPlayer);
        rb.AddForce(grabbedTransform.parent.transform.forward * throwSpeed, ForceMode.Impulse);
        thrown = true;
    }


    private void OnCollisionEnter(Collision collision){
        if(thrown){
            if(collision.gameObject.layer == 6 || collision.gameObject.layer == 10){
                // break or effect
                ObjectEffect();
            }
        }
    }


    // override with new script
    public virtual void ObjectEffect(){
        if(effectActivated) return;

        Debug.Log("thrown object collided");
        //GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Player", "Object"); // change later
        effectActivated = true;
        //hitbox.hit = true;  // change later
        //Destroy(this.gameObject);
    }


    public void ShowHideObject(bool show){
        GetComponent<MeshRenderer>().enabled = show;
    }


    public void DisableObject(){
        if(objectDisabled) return;

        GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Player", "Object");
        hitbox.hit = true;
        objectDisabled = true;
        Debug.Log("object disabled");
    }
}
