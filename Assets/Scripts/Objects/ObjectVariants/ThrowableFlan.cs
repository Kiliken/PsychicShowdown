using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableFlan : ThrowableObject
{
    PlayerMovement jumpingPlayer;
    [SerializeField] protected LayerMask jumpCollisionMask;


    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (!thrown)
        {
            if (collision.gameObject.tag == "Player1" || collision.gameObject.tag == "Player2")
            {
                // only make player jump if they are colliding on the top
                if (collision.gameObject.transform.position.y > transform.position.y + 2)
                {
                    jumpingPlayer = collision.gameObject.GetComponent<PlayerMovement>();
                    jumpingPlayer.Jump(true);
                    jumpingPlayer.jumpsLeft = jumpingPlayer.maxJumps;
                    sfxPlayer.PlaySFX(0);
                }

            }
        }
    }
}
