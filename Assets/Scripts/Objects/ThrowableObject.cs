using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    protected ObjSFXPlayer sfxPlayer;
    protected GameObject model;
    protected GameObject highlightEffect;
    protected Rigidbody rb;
    protected ObjHitbox hitbox;
    public int objectID = 0;    // ID for showing object on UI, etc.
    public string objectName = "Object (size)";
    [SerializeField] public int objectSize = 0; // 0:small, 1:medium, 2:large
    public bool canGrab = true;
    protected bool grabbed = false;
    public bool canThrow = false;
    public bool aiming = false;
    protected bool thrown = false;
    protected bool effectActivated = false;
    // DAMAGES
    protected int[] damages = new int[] { 1, 2, 3 }; // S, M, L
    [SerializeField] public int damage = 1;
    [SerializeField] protected bool objSpecificDmg = false;

    // THROW SPEEDS
    protected float[] throwSpeeds = new float[] { 120f, 80f, 50f }; // S, M, L
    [SerializeField] protected float throwSpeed = 120f;
    // GRAB SPEEDS
    //protected float[] grabSpeeds = new float[]{30f, 20f, 10f}; // S, M, L 30 20 10
    protected float[] grabSpeeds = new float[] { 0.7f, 0.5f, 0.4f }; // S, M, L 30 20 10
    [SerializeField] protected float grabSpeed = 0.7f;
    [SerializeField] protected bool objSpecificSpeed = false;


    public Transform grabbedTransform;
    public Transform shootPos;
    public int holdingPlayer = 0;
    [SerializeField] protected Vector3 grabbedRotation;
    [SerializeField] protected Vector3 shootRotation;

    [SerializeField] protected float disableHitboxVelo = 20f;    // the magnitude of the velocity to disable the hitbox when thrown
    protected bool objectDisabled = false;
    [SerializeField] protected float destroyAfterSec = 3f;
    protected float destroyTimer = 0f;
    [SerializeField] protected GameObject effectParticle;
    protected bool objectVisible = true;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        model = transform.GetChild(1).gameObject;
        rb = GetComponent<Rigidbody>();
        hitbox = transform.GetChild(0).GetComponent<ObjHitbox>();
        if(transform.Find("Effect"))
            highlightEffect = transform.Find("Effect").gameObject;
        ShowHideHighlight(false);
        sfxPlayer = GetComponent<ObjSFXPlayer>();
        if(!objSpecificDmg)
            damage = damages[objectSize];
        if (!objSpecificSpeed)
        {
            throwSpeed = throwSpeeds[objectSize];
            grabSpeed = grabSpeeds[objectSize];
        }
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        // aim
        if (aiming)
        {
            transform.position = shootPos.position;
            transform.rotation = grabbedTransform.parent.rotation * Quaternion.Euler(shootRotation.x, shootRotation.y, shootRotation.z);
            if (objectVisible)
                ShowHideObject(false, true);
        }
        else if (!thrown && !canGrab)
        {
            // grab
            if (!grabbed && Vector3.Distance(transform.position, grabbedTransform.position) > 0.1f)
            {
                //transform.position = Vector3.Lerp(transform.position, grabbedTransform.position, grabSpeed * Time.deltaTime);
            }
            // snap to player
            else
            {
                transform.position = grabbedTransform.position;
                transform.rotation = grabbedTransform.parent.rotation * Quaternion.Euler(grabbedRotation.x, grabbedRotation.y, grabbedRotation.z);
                canThrow = true;
                grabbed = true;
            }
        }
        // if thrown and low velocity, disable hitbox and collider
        else if (thrown && rb.velocity.magnitude > 0 && rb.velocity.magnitude < disableHitboxVelo)
        {
            DisableObject();
        }
        // set object to be destoryed after collision/effect activation
        if (effectActivated)
        {
            if (destroyTimer < destroyAfterSec)
            {
                destroyTimer += Time.deltaTime;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        //Debug.Log(rb.velocity.magnitude);
    }

    void FixedUpdate()
    {
        if (!thrown && !canGrab)
        {
            // grab
            if (!grabbed && Vector3.Distance(transform.position, grabbedTransform.position) > 0.1f)
            {
                rb.MovePosition(Vector3.MoveTowards(transform.position, grabbedTransform.position, grabSpeed));
            }
        }
    }


    public void GrabObject(Transform posTransform, Transform shootTransform, int player)
    {
        if (objectSize == 2)
        {
            rb.isKinematic = false;
            model.GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Object");
        }

        grabbedTransform = posTransform;
        shootPos = shootTransform;
        holdingPlayer = player;
        rb.useGravity = false;
        model.GetComponent<MeshCollider>().enabled = false;
        ShowHideHighlight(false);
        canGrab = false;
    }


    public virtual void ThrowObject()
    {
        aiming = false;
        rb.useGravity = true;

        ShowHideObject(true, true);
        //model.GetComponent<MeshRenderer>().enabled = true;
        model.GetComponent<MeshCollider>().enabled = true;
        model.GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask();   // remove layer mask exclusions
        // activate hit box
        hitbox.ActivateHitbox(holdingPlayer);
        rb.AddForce(grabbedTransform.parent.transform.forward * throwSpeed, ForceMode.Impulse);
        if (highlightEffect)
            highlightEffect.SetActive(false);
        thrown = true;
    }


    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (thrown)
        {
            if (collision.gameObject.layer == 6 || collision.gameObject.layer == 10)
            {
                // break or effect
                ObjectEffect();
            }
        }
    }


    // override with new script
    public virtual void ObjectEffect()
    {
        if (effectActivated) return;

        Debug.Log("thrown object collided");
        //GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Player", "Object"); // change later

        // play sfx
        sfxPlayer.PlaySFX(0);

        // instantiate hit effect
        if (effectParticle != null)
        {
            Instantiate(effectParticle, transform.position, quaternion.identity);
        }
        effectActivated = true;
        //hitbox.hit = true;  // change later
        //Destroy(this.gameObject);
    }


    public void ShowHideObject(bool show, bool culling)
    {
        //model.GetComponent<MeshRenderer>().enabled = show;
        if (!culling)
        {
            model.GetComponent<MeshRenderer>().enabled = show;
            //if (highlightEffect)
            //    highlightEffect.GetComponent<MeshRenderer>().enabled = show;
        }
        else
        {
            if (!show)
            {
                if (holdingPlayer == 1)
                {
                    model.layer = LayerMask.NameToLayer("P1Hide");
                    if (highlightEffect)
                        highlightEffect.layer = LayerMask.NameToLayer("P1Hide");
                }
                else
                {
                    model.layer = LayerMask.NameToLayer("P2Hide");
                    if (highlightEffect)
                        highlightEffect.layer = LayerMask.NameToLayer("P2Hide");
                }
            }
            else
            {
                model.layer = LayerMask.NameToLayer("Object");
                if (highlightEffect)
                    highlightEffect.layer = LayerMask.NameToLayer("Object");
            }
        }


        objectVisible = show;
    }


    public void DisableObject()
    {
        if (objectDisabled) return;

        model.GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Player", "Object");
        hitbox.hit = true;
        objectDisabled = true;
        Debug.Log("object disabled");
    }

    public void ShowHideHighlight(bool show)
    {
        if(!highlightEffect) return;
        highlightEffect.GetComponent <MeshRenderer>().enabled = show;
    }
}
