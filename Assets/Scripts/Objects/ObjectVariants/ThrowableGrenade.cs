using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ThrowableGrenade : ThrowableObject
{
    ExplosionHitbox explosionHitbox;
    [SerializeField] float explodeTime = 2f;
    private float explodeTimer = 0f;
    public int explosionDamage = 1;
    private bool hit = false;

    protected override void Start()
    {
        base.Start();
        explosionHitbox = transform.GetChild(2).GetComponent<ExplosionHitbox>();
    }

    protected override void Update()
    {
        base.Update();
        if (hit)
        {
            if (explodeTimer < explodeTime)
            {
                explodeTimer += Time.deltaTime;
            }
            else
            {
                Explode();
                hit = false;
            }
        }
    }


    // add object effects here
    public override void ObjectEffect()
    {
        if (effectActivated) return;

        Debug.Log("grenade hit");
        sfxPlayer.PlaySFX(0);
        hit = true;
        effectActivated = true;
        //Destroy(this.gameObject);
    }

    private void Explode()
    {
        if (effectParticle != null)
        {
            Instantiate(effectParticle, transform.position, quaternion.identity);
        }
        model.GetComponent<MeshRenderer>().enabled = false;
        explosionHitbox.ActivateHitbox();
        sfxPlayer.PlaySFX(1);

    }
}
