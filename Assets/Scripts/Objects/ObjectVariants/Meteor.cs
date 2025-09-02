using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Meteor : ThrowableObject
{
    private Vector3 explosionOffset = new Vector3(0, -7f, 0);

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ThrowObject();
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
        rb.AddForce(-transform.up * throwSpeed, ForceMode.Impulse);
        if (highlightEffect)
            highlightEffect.SetActive(false);
        thrown = true;
    }


    // override with new script
    public override void ObjectEffect()
    {
        if (effectActivated) return;

        Debug.Log("thrown object collided");
        //GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Player", "Object"); // change later

        // play sfx
        sfxPlayer.PlaySFX(0);

        // instantiate hit effect
        if (effectParticle != null)
        {
            Instantiate(effectParticle, transform.position + explosionOffset, quaternion.identity);
        }
        effectActivated = true;
        //Destroy(this.gameObject);
    }
}
