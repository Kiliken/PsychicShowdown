using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SpearHitbox : ObjHitbox
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (hit) return;

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

            transform.parent.GetComponent<ThrowableSpear>().otherCollider = other;
            transform.parent.GetComponent<ThrowableSpear>().contactPoint = GetComponent<Collider>().ClosestPoint(other.transform.position);
            transform.parent.GetComponent<ThrowableSpear>().ObjectEffect();

            other.gameObject.transform.parent.GetComponent<Player>().ReceiveDamage(objectScript.damage);
            hit = true;
            GetComponent<BoxCollider>().enabled = false;
        }

    }
}
