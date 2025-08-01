using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableSpear : ThrowableObject
{
    [SerializeField] private GameObject childObject;
    public bool isChild = false;


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


    private void CloneObjects()
    {
        Vector3[] pos = {new Vector3(transform.position.x, transform.position.y + 2, transform.position.z),
        new Vector3(transform.position.x + 2, transform.position.y + 2, transform.position.z),
        new Vector3(transform.position.x - 2, transform.position.y + 2, transform.position.z),
        new Vector3(transform.position.x, transform.position.y + 4, transform.position.z),
        new Vector3(transform.position.x + 2, transform.position.y + 4, transform.position.z),
        new Vector3(transform.position.x - 2, transform.position.y + 4, transform.position.z),
        new Vector3(transform.position.x - 4, transform.position.y + 4, transform.position.z),
        new Vector3(transform.position.x + 4, transform.position.y + 4, transform.position.z)
        };

        for (int i = 0; i < pos.Length; i++)
        {
            GameObject spear = Instantiate(childObject, pos[i], transform.rotation);
            spear.GetComponent<ThrowableSpear>().holdingPlayer = holdingPlayer;
            spear.GetComponent<ThrowableSpear>().canGrab = false;
            spear.GetComponent<ThrowableSpear>().isChild = true;
        }

    }
}
