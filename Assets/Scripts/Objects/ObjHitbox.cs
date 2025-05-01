using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjHitbox : MonoBehaviour
{
    private int holdingPlayer = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ActivateHitbox(int player){
        holdingPlayer = player;
        GetComponent<BoxCollider>().enabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        // may need to change tags to hurtbox later
        // damage player 1
        if(other.gameObject.tag == "Player1" && holdingPlayer == 2){
            // apply damage
            Debug.Log("Hit player 1");
            transform.parent.GetComponent<ThrowableObject>().ObjectEffect();
        }
        else if(other.gameObject.tag == "Player2" && holdingPlayer == 1){
            // apply damage
            Debug.Log("Hit player 2");
            transform.parent.GetComponent<ThrowableObject>().ObjectEffect();
        }
    }
}
