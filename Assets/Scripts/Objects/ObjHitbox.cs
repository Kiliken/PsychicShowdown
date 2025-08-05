using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjHitbox : MonoBehaviour
{
    protected ThrowableObject objectScript;
    protected int holdingPlayer = 0;
    public bool hit = false;


    // Start is called before the first frame update
    protected void Start()
    {
        objectScript = transform.parent.GetComponent<ThrowableObject>();
    }

    // Update is called once per frame
    protected void Update()
    {

    }


    public void ActivateHitbox(int player)
    {
        holdingPlayer = player;
        GetComponent<BoxCollider>().enabled = true;
    }


    protected virtual void OnTriggerEnter(Collider other)
    {
        if (hit) return;

        // if(other.gameObject.tag == "Player1" && holdingPlayer == 2){
        //     // apply damage
        //     Debug.Log("Hit player 1");
        //     transform.parent.GetComponent<ThrowableObject>().ObjectEffect();
        //     hit = true;
        //     GetComponent<BoxCollider>().enabled = false;
        // }
        // else if(other.gameObject.tag == "Player2" && holdingPlayer == 1){
        //     // apply damage
        //     Debug.Log("Hit player 2");
        //     transform.parent.GetComponent<ThrowableObject>().ObjectEffect();
        //     hit = true;
        //     GetComponent<BoxCollider>().enabled = false;
        // }

        // may need to change tags to hurtbox later
        if ((other.gameObject.tag == "Player1" && holdingPlayer == 2) || (other.gameObject.tag == "Player2" && holdingPlayer == 1))
        {
            if (holdingPlayer == 2)
            {
                Debug.Log("Hit player 1 " + other.gameObject.name);
                // apply damage to player 1
            }
            else if (holdingPlayer == 1)
            {
                Debug.Log("Hit player 2 " + other.gameObject.name);
                // apply damage to player 2
            }

            other.gameObject.transform.parent.GetComponent<Player>().ReceiveDamage(objectScript.damage);
            objectScript.ObjectEffect();
            objectScript.DisableObject();
            hit = true;
            GetComponent<BoxCollider>().enabled = false;
        }

    }
}
