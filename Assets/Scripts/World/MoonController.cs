using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonController : MonoBehaviour
{
    Transform moon;

    private void Start()
    {
        moon = transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(0, Time.deltaTime * 2f, 0));
        moon.Rotate(new Vector3(0, Time.deltaTime * 5f, 0));
    }
}
