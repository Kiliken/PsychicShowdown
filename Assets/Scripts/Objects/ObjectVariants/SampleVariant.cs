using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleVariant : ThrowableObject
{
    // add object effects here
    public override void ObjectEffect(){
        if(effectActivated) return;
        
        base.ObjectEffect();    // base method
        Debug.Log("Variant effect");
        //Destroy(this.gameObject);
    }
}
