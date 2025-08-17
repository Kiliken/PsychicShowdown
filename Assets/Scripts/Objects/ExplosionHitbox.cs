using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHitbox : MonoBehaviour
{
    ThrowableGrenade objectScript;
    public bool hit = false;


    // Start is called before the first frame update
    void Start()
    {
        objectScript = transform.parent.GetComponent<ThrowableGrenade>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ActivateHitbox()
    {
        GetComponent<SphereCollider>().enabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (hit) return;

        // may need to change tags to hurtbox later
        if (other.gameObject.tag == "Player1" || other.gameObject.tag == "Player2")
        {
            if (other.gameObject.transform.parent.GetComponent<Player>() != null)
            {
                other.gameObject.transform.parent.GetComponent<Player>().ReceiveDamage(objectScript.explosionDamage);
            }
            hit = true;
            GetComponent<SphereCollider>().enabled = false;
        }

    }
}
