using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ThrowableMeteor : ThrowableObject
{
    [SerializeField] GameObject meteorPrefab;
    [HideInInspector] public Vector3 contactPoint;
    private float spawnOffset = 100f;


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

        SpawnMeteor();

        // instantiate hit effect
        if (effectParticle != null)
        {
            Instantiate(effectParticle, transform.position, quaternion.identity);
        }
        effectActivated = true;
        //hitbox.hit = true;  // change later
        //Destroy(this.gameObject);
    }


    public void SpawnMeteor()
    {
        GameObject meteor = Instantiate(meteorPrefab, transform.position + Vector3.up * spawnOffset, quaternion.identity);
        meteor.GetComponent<Meteor>().holdingPlayer = holdingPlayer;
        meteor.GetComponent<Meteor>().canGrab = false;

    }
}
