using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ThrowableSkull : ThrowableObject
{
    Transform player1Pos;
    Transform player2Pos;
    Transform targetPos;


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
        // else if (thrown && rb.velocity.magnitude > 0 && rb.velocity.magnitude < disableHitboxVelo && !effectActivated)
        // {
        //     DisableObject();
        // }
        // set object to be destoryed after collision/effect activation
        if (effectActivated)
        {
            StartCoroutine(Dissolve());

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


    protected override void FixedUpdate()
    {
        if (!thrown && !canGrab)
        {
            // grab
            if (!grabbed && Vector3.Distance(transform.position, grabbedTransform.position) > 0.1f)
            {
                rb.MovePosition(Vector3.MoveTowards(transform.position, grabbedTransform.position, grabSpeed));
            }
        }
        else if (thrown && !effectActivated)
        {
            if (Vector3.Distance(transform.position, grabbedTransform.position) > 0.5f)
            {
                rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos.position, 2f));
            }
        }
    }


    public override void ObjectEffect()
    {
        if (effectActivated) return;

        rb.useGravity = true;

        Debug.Log("skull hit");

        sfxPlayer.PlaySFX(0);
        if (effectParticle != null)
        {
            Instantiate(effectParticle, transform.position, quaternion.identity);
        }

        effectActivated = true;
        //Destroy(this.gameObject);
    }

    public override void ThrowObject()
    {
        aiming = false;
        //rb.useGravity = true;

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

    public override void GrabObject(Transform posTransform, Transform shootTransform, int player)
    {
        base.GrabObject(posTransform, shootTransform, player);
        if (holdingPlayer == 1)
            targetPos = player2Pos;
        else
            targetPos = player1Pos;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (thrown)
        {
            Debug.Log(collision.gameObject.name);
            if (((1 << collision.gameObject.layer) & rbCollisionMask.value) != 0)
            {
                // break or effect
                ObjectEffect();
            }
        }
    }
}
