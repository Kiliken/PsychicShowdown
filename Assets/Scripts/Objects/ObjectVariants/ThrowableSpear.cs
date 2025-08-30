using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ThrowableSpear : ThrowableObject
{
    [SerializeField] private GameObject childObject;
    public bool isChild = false;
    [HideInInspector] public Vector3 contactPoint;
    [HideInInspector] public Collider otherCollider;
    private float spearOffset = 4f;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (isChild)
            ThrowObject();
    }


    // Update is called once per frame
    // protected override void Update()
    // {

    // }


    public override void ThrowObject()
    {
        aiming = false;
        rb.useGravity = true;

        ShowHideObject(true, true);
        //model.GetComponent<MeshRenderer>().enabled = true;
        model.GetComponent<MeshCollider>().enabled = true;
        //model.GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask();   // remove layer mask exclusions
        // activate hit box
        hitbox.ActivateHitbox(holdingPlayer);
        rb.AddForce(grabbedTransform.parent.transform.forward * throwSpeed, ForceMode.Impulse);
        if (highlightEffect)
            highlightEffect.SetActive(false);
        thrown = true;
        if (!isChild)
            CloneObjects();
    }

    public override void ThrowObjectNet()
    {
        aiming = false;
        rb.useGravity = true;

        ShowHideObject(true, true);
        //model.GetComponent<MeshRenderer>().enabled = true;
        model.GetComponent<MeshCollider>().enabled = true;
        //model.GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask();   // remove layer mask exclusions
        // activate hit box
        hitbox.ActivateHitbox(holdingPlayer);

        rb.position = shootPos.position + holdPosPadding;
        rb.rotation = shootPos.rotation * Quaternion.Euler(shootRotation.x, shootRotation.y, shootRotation.z);

        rb.AddForce(shootPos.forward * throwSpeed, ForceMode.Impulse);


        if (highlightEffect)
            highlightEffect.SetActive(false);
        thrown = true;

        if (!isChild)
            CloneObjects();
    }


    private void CloneObjects()
    {
        // transform axes
        Vector3 up = transform.up;
        Vector3 right = transform.right;

        // local-space offsets
        Vector3[] localOffsets = {
            up * 2,
            right * 2 + up * 2,
            -right * 2 + up * 2,
            up * 4,
            right * 2 + up * 4,
            -right * 2 + up * 4,
            -right * 4 + up * 4,
            right * 4 + up * 4
        };

        // instantiate spears relative to orientation
        foreach (var offset in localOffsets)
        {
            // basePos + offset to get world pos
            GameObject spear = Instantiate(childObject, transform.position + offset, transform.rotation);
            spear.GetComponent<ThrowableSpear>().holdingPlayer = holdingPlayer;
            spear.GetComponent<ThrowableSpear>().canGrab = false;
            spear.GetComponent<ThrowableSpear>().isChild = true;
        }
    }


    protected override void OnCollisionEnter(Collision collision)
    {
        if (thrown)
        {
            if (((1 << collision.gameObject.layer) & rbCollisionMask.value) != 0)
            {
                // store contact point
                ContactPoint point = collision.contacts[0];
                // break or effect
                if (collision.contactCount > 0)
                {
                    contactPoint = point.point;
                    otherCollider = point.otherCollider;
                    ObjectEffect();
                }

            }
        }
    }


    public override void ObjectEffect()
    {
        if (effectActivated) return;

        Debug.Log("thrown object collided");
        //GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Player", "Object"); // change later

        // play sfx
        sfxPlayer.PlaySFX(0);

        // stop velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Stick spear at the point of contact
        StickSpear();

        // instantiate hit effect
        // if (effectParticle != null)
        // {
        //     Instantiate(effectParticle, transform.position, quaternion.identity);
        // }
        effectActivated = true;
        //hitbox.hit = true;  // change later
        //Destroy(this.gameObject);
    }


    void StickSpear()
    {
        // offset from the tip of the spear
        Vector3 tipOffset = transform.forward * spearOffset;

        // move spear so the tip lands on the contact point
        transform.position = contactPoint - tipOffset;

        // align the spear to the normal of the surface
        //transform.rotation = Quaternion.LookRotation(-contact.normal);

        // parent the spear to the hit object

        Debug.Log(otherCollider.name);
        transform.SetParent(otherCollider.transform);

        DisableObject();

    }
}
