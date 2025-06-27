using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableSkull : ThrowableObject
{
    Transform player1Pos;
    Transform player2Pos;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        player1Pos = GameObject.FindGameObjectWithTag("Player1").transform;
        player2Pos = GameObject.FindGameObjectWithTag("Player2").transform;
    }

    // Update is called once per frame
    protected override void Update()
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

    public override void ObjectEffect()
    {
        if (effectActivated) return;

        Debug.Log("skull hit");
        sfxPlayer.PlaySFX(0);
        effectActivated = true;
        //Destroy(this.gameObject);
    }

    public override void ThrowObject()
    {
        aiming = false;
        rb.useGravity = true;

        ShowHideObject(true, true);
        //model.GetComponent<MeshRenderer>().enabled = true;
        model.GetComponent<MeshCollider>().enabled = true;
        model.GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask();   // remove layer mask exclusions
        // activate hit box
        hitbox.ActivateHitbox(holdingPlayer);
        //rb.AddForce(grabbedTransform.parent.transform.forward * throwSpeed, ForceMode.Impulse);
        if (highlightEffect)
            highlightEffect.SetActive(false);
        thrown = true;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
